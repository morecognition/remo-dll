using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace morecognition
{
    internal class Device
    {
        private string _comName;
        private SerialPort _serial;
        private CancellationTokenSource _tokenSource;
        public event EventHandler<PacketsArgs> PacketReceived;
        private Queue<Action> _actions;
        private int packetLen;
        private int packetDataLen;
        private ATcommand.ACQmode acqMode;
        private MyTaskScheduler _scheduler;
        private int RXcount;
        private int parseErrCount;
        private int syncErrCount;
        private DateTime _firstTimestamp;
        bool devStatus;

        public Device()
        {
            _serial = new SerialPort();
            _actions = new Queue<Action>();
            devStatus = false;
        }

        public bool IsConnected()
        {
            return devStatus;
        }

        public string ComName
        {
            get
            {
                return this._comName;
            }
            set
            {
                this._comName = value;
            }
        }

        public DateTime FirstTimestamp { get => _firstTimestamp; set => _firstTimestamp = value; }
        public SerialPort Serial => _serial;

        public async Task Start(ATcommand.ACQmode mode)
        {
            if (!String.IsNullOrEmpty(_comName))
            {
                acqMode = mode;
                _serial.PortName = _comName;
                //_serial.ReadTimeout = 200;
                _serial.ReadBufferSize = 40960;

                if (!_serial.IsOpen)
                {
                    _serial.Open();
                    _serial.DiscardInBuffer();
                    _serial.DiscardOutBuffer();
                    _actions.Enqueue(() => ATcommandExecution.SetAcquisitionmode(this, mode));
                    _actions.Enqueue(() => ATcommandExecution.SetOperatingMode(this, ATcommand.OpMode.TxMode));

                    _tokenSource = new CancellationTokenSource();
                    _scheduler = new MyTaskScheduler(_tokenSource.Token);
                    _scheduler.Start();

                    await ReadDataAsync(_tokenSource.Token);
                }
            }
        }

        public Task Stop()
        {
            Task closeTask = new Task(() =>
            {
                try
                {
                    _tokenSource?.Cancel();
                    if (_serial != null && _serial.IsOpen)
                    {
                        _serial.Close();
                    }
                }
                catch (IOException)
                {

                }
            });
            closeTask.Start();

            return closeTask;
        }
        public void EnableMotor()
        {
            ATcommandExecution.SetMotor(_serial, ATcommand.MotorMode.Enabled);
        }

        public void DisableMotor()
        {
            ATcommandExecution.SetMotor(_serial, ATcommand.MotorMode.Disabled);
        }


        private async Task ReadDataAsync(CancellationToken token)
        {
            foreach (Action action in _actions)
            {
                action.Invoke();
            }

            RXcount = 0;
            parseErrCount = 0;
            syncErrCount = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int toBeRead = 0;
            int mult;
            List<byte[]> buff_list = new List<byte[]>();

            if (acqMode == ATcommand.ACQmode.RAW)
            {
                mult = 40;
                packetLen = 23;
            }
            else
            {
                mult = 1;
                packetLen = 41;
            }

            // The first 7 bytes are an indicator in the start %, 2 null bytes and 4 bytes for time stamp
            // The rest are realted to data
            packetDataLen = packetLen - 7;
            DateTime start_msg = new DateTime();
            while (!token.IsCancellationRequested)
            {
                byte[] buffer = new byte[packetLen * mult];
                Task<int> readStringTask = _serial.BaseStream.ReadAsync(buffer, 0, packetLen * mult, token);

                int bytesRead = 0;
                try
                {
                    bytesRead = await readStringTask;
                }
                catch (Exception)
                { }

                switch (acqMode)
                {
                    case ATcommand.ACQmode.RMS:
                    case ATcommand.ACQmode.RAW_IMU:
                        _scheduler.Schedule(() => {
                            ManageRMSmessage(buffer.ToList());
                        });
                        break;
                    case ATcommand.ACQmode.RAW:
                        if (buff_list.Count == 0)
                            start_msg = DateTime.Now;

                        buff_list.Add(buffer);
                        break;
                }

                if (_serial.IsOpen)
                {
                    toBeRead = _serial.BytesToRead;
                }

                // Console.WriteLine(toBeRead +" "+ buff_list.Count);   
            }

            sw.Stop();
            if (acqMode == ATcommand.ACQmode.RAW)
            {
                ManageRAWmessage(buff_list, start_msg);
            }

            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine($"Dispositivo: {ComName}");
            Console.WriteLine("Ricevuti " + RXcount + " pacchetti in " + elapsedTime);
            Console.WriteLine("Ricevuti " + string.Format("{0:N3}", (double)RXcount / sw.Elapsed.TotalSeconds) + " pacchetti/s");
            Console.WriteLine("Da leggere: " + toBeRead);
            Console.WriteLine("Parsing err: " + parseErrCount + ", Sync err: " + syncErrCount);
        }

        private void ManageRMSmessage(List<byte> buffer)
        {
            if (ParseMessage(buffer, out Sample s))
            {
                List<Sample> myList = new List<Sample>();
                myList.Add(s);
                OnPacketReceived(new PacketsArgs(myList));
                RXcount++;
            }
        }

        private void ManageRAWmessage(List<byte[]> buff_list, DateTime start_msg)
        {
            List<byte> mega_buff = new List<byte>();
            foreach (byte[] buff in buff_list)
            {
                mega_buff.AddRange(buff);
            }
            string hex = BitConverter.ToString(mega_buff.ToArray());
            hex = hex.Replace("-", ".");

            int index = 0;
            bool reading = false;
            List<Sample> s_list = new List<Sample>();
            List<byte> message_byte = new List<byte>();
            while (index < mega_buff.Count)
            {
                if (mega_buff[index] == '%' && reading == false)
                {
                    message_byte.Add(mega_buff[index]);
                    reading = true;
                }
                else
                {
                    if (reading)
                    {
                        message_byte.Add(mega_buff[index]);
                        if (mega_buff[index] == '&' && message_byte.Count == packetLen)
                        {
                            reading = false;
                            if (ParseMessage(message_byte, out Sample s))
                            {
                                s_list.Add(s);

                                //if (s_list.Count > 1)
                                //{
                                //    UInt16 diff = (UInt16)(s_list[s_list.Count - 1].counter - s_list[s_list.Count - 2].counter);
                                //    start_msg = start_msg.AddMilliseconds(diff);
                                //    s.date = start_msg;

                                //    if (s_list[s_list.Count - 1].counter != (s_list[s_list.Count - 2].counter + 1))
                                //    {
                                //        syncErrCount++;
                                //    }
                                //}
                                //else
                                //    s.date = start_msg;

                                RXcount++;
                            }
                            else
                            {
                                parseErrCount++;
                            }
                            message_byte = new List<byte>();
                        }
                        else
                        {
                            if (mega_buff[index] == '&' && message_byte.Count > packetLen)
                            {
                                if (message_byte[0] == '%' && message_byte[message_byte.Count - 1] == '&')
                                {
                                    List<byte> recover = new List<byte>();
                                    recover.Add(message_byte[0]);
                                    for (int ix = 0; ix < packetLen - 1; ix++)
                                    {
                                        recover.Add(message_byte[message_byte.Count - packetLen + ix + 1]);
                                    }

                                    if (recover.Count == packetLen)
                                    {
                                        if (ParseMessage(recover, out Sample s))
                                        {
                                            s_list.Add(s);

                                            //if (s_list.Count > 1)
                                            //{
                                            //    UInt16 diff = (UInt16)(s_list[s_list.Count - 1].counter - s_list[s_list.Count - 2].counter);
                                            //    start_msg = start_msg.AddMilliseconds(diff);
                                            //    s.date = start_msg;

                                            //    if (s_list[s_list.Count - 1].counter != (s_list[s_list.Count - 2].counter + 1))
                                            //    {
                                            //        syncErrCount++;
                                            //    }
                                            //}
                                            //else
                                            //    s.date = start_msg;

                                            RXcount++;
                                        }
                                        else
                                        {
                                            parseErrCount++;
                                        }
                                    }
                                }
                                reading = false;
                                message_byte = new List<byte>();
                            }
                        }
                    }
                }
                index++;
            }

            OnPacketReceived(new PacketsArgs(s_list));
        }

        private void OnPacketReceived(PacketsArgs e)
        {
            EventHandler<PacketsArgs> handler = PacketReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool ParseMessage(List<byte> message, out Sample campione)
        {
            bool ret_val = false;

            campione = new Sample();
            if (message.Count == packetLen)
            {
                if (message[0] == '%' && message[message.Count - 1] == '&')
                {
                    byte myCRC = 0;
                    int index = 0;
                    foreach (byte b in message)
                    {
                        if (index != message.Count - 2)
                        {
                            myCRC = (byte)(myCRC ^ b);
                        }
                        index++;
                    }
                    if (myCRC == message[message.Count - 2])
                    {
                        message.RemoveAt(0);
                        message.RemoveAt(message.Count - 1);
                        message.RemoveAt(message.Count - 1);

                        campione.total_decimilliseconds = (UInt32)(((UInt32)message[0] << 24) | ((UInt32)message[1] << 16) | ((UInt32)message[2] << 8) | (UInt32)message[3]);
                        message.RemoveAt(0);
                        message.RemoveAt(0);
                        message.RemoveAt(0);
                        message.RemoveAt(0);

                        if (message.Count == packetDataLen)
                        {
                            //if (_firstTimestamp.Year == 1)
                            //{
                            //    _firstTimestamp = DateTime.Now;
                            //    campione.date = _firstTimestamp;
                            //}
                            //else

                            campione.date = _firstTimestamp.AddMilliseconds(campione.total_decimilliseconds / 10.0);

                            for (byte i = 0; i < 8; i++)
                            {
                                Int16 value = (Int16)((UInt16)message[(i * 2)] << 8 | (UInt16)message[(i * 2) + 1]);
                                campione.measure.Add((byte)(i + 1), value);
                            }

                            message.RemoveRange(0, 16);

                            if (acqMode == ATcommand.ACQmode.RMS || acqMode == ATcommand.ACQmode.RAW_IMU)
                            {
                                for (byte i = 0; i < 3; i++)
                                {
                                    Int16 value = (Int16)((UInt16)message[(i * 2)] << 8 | (UInt16)message[(i * 2) + 1]);
                                    float valueF = ((float)value) / 100;
                                    switch (i)
                                    {
                                        case 0:
                                            campione.acc.x = valueF;
                                            break;
                                        case 1:
                                            campione.acc.y = valueF;
                                            break;
                                        case 2:
                                            campione.acc.z = valueF;
                                            break;
                                    }
                                }

                                message.RemoveRange(0, 6);

                                for (byte i = 0; i < 3; i++)
                                {
                                    Int16 value = (Int16)((UInt16)message[(i * 2)] << 8 | (UInt16)message[(i * 2) + 1]);
                                    float valueF = ((float)value) / 100;
                                    switch (i)
                                    {
                                        case 0:
                                            campione.gyro.x = valueF;
                                            break;
                                        case 1:
                                            campione.gyro.y = valueF;
                                            break;
                                        case 2:
                                            campione.gyro.z = valueF;
                                            break;
                                    }
                                }

                                message.RemoveRange(0, 6);

                                for (byte i = 0; i < 3; i++)
                                {
                                    Int16 value = (Int16)((UInt16)message[(i * 2)] << 8 | (UInt16)message[(i * 2) + 1]);
                                    float valueF = ((float)value) / 100;
                                    switch (i)
                                    {
                                        case 0:
                                            campione.mag.x = valueF;
                                            break;
                                        case 1:
                                            campione.mag.y = valueF;
                                            break;
                                        case 2:
                                            campione.mag.z = valueF;
                                            break;
                                    }
                                }
                            }

                            ret_val = true;
                        }
                    }
                }
            }

            return ret_val;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
    }
}
