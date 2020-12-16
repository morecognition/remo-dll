using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;


namespace morecognition
{
    public class MatlabCOM
    {
        private Device dev;
        //public List<string> Last_packet = new List<string>();
        bool readFlag = false;
        double EMG_resolution = 0.0238; //uV
        private readonly Object stringLock = new Object();
        String last_packet;

        public void StartListening(ref int COM_num, ref int mode)
        {
            int m = mode;
            dev = new Device();
            dev.ComName = (string)("COM" + COM_num);
            dev.PacketReceived += Dev_PacketReceived;
            //Last_packet.Clear();
            Last_packet = String.Empty;
            var t = Task.Run(async () =>
            {
                try
                {
                    switch (m)
                    {
                        case 1:
                            await dev.Start(ATcommand.ACQmode.RMS);
                            break;
                        case 2:
                            await dev.Start(ATcommand.ACQmode.RAW_IMU);
                            break;
                    }
                }
                catch
                {
                    return;
                }
            });
        }

        public String Last_packet
        {
            get { return last_packet; }
            set
            {
                lock (stringLock)
                {
                    last_packet = value;
                }
            }
        }

        public async void StopListening()
        {
            if (dev != null)
            {
                await dev.Stop();
                //Last_packet.Clear();
            }
        }

        public string ReadPacket()
        {
            //string StrToSend = string.Empty;

            readFlag = true;

            //if (Last_packet.Count > 0)
            //{
            //    lock (stringLock)
            //    {
            //        foreach (string srt in Last_packet)
            //        {
            //            StrToSend += srt + '&';
            //        }

            //        readFlag = true;
            //    }
            //}
            return Last_packet;
        }

        private void Dev_PacketReceived(object sender, PacketsArgs e)
        {
            string temp = string.Empty;

            if (readFlag == true)
            {
                readFlag = false;
                //Last_packet.Clear();
                Last_packet = String.Empty;
            }

            foreach (Sample point in e.packets)
            {
                temp = string.Empty;
                foreach (byte ch in point.measure.Keys)
                {
                    temp += point.measure[ch] * EMG_resolution + ";";
                }

                temp += point.acc.x + ";";
                temp += point.acc.y + ";";
                temp += point.acc.z + ";";

                temp += point.gyro.x + ";";
                temp += point.gyro.y + ";";
                temp += point.gyro.z + ";";

                temp += point.mag.x + ";";
                temp += point.mag.y + ";";
                temp += point.mag.z + ";";

                //if (Last_packet.Count >= 2)
                //{
                //    Last_packet.RemoveAt(0);
                //}


                //Last_packet.Add(temp);
                //Last_packet.Add(temp);
                Last_packet += temp + '&';
            }





        }


    }
}
