using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static morecognition.ATcommand;

namespace morecognition
{
    internal class ATcommandExecution
    {
        private static ATcommand ReadAnswer(SerialPort com)
        {
            ATcommand reply = null;

            List<byte> receivedData = new List<byte>();
            byte bait = 0;
            bool timeout = false;

            DateTime start = DateTime.Now;
            while (bait != (byte)ATcommand.CR_LF_BYTE.AT_COMMAND_LF)
            {
                try
                {
                    bait = (byte)com.ReadByte();
                    receivedData.Add(bait);

                    if (((TimeSpan)(DateTime.Now - start)).TotalMilliseconds > com.ReadTimeout)
                    {
                        throw new TimeoutException();
                    }
                }
                catch (TimeoutException)
                {
                    timeout = true;
                    break;
                }
            }

            if (!timeout)
            {
                reply = new ATcommand();
                if (reply.ProcessReply(receivedData) == false)
                    reply = null;
            }

            return reply;
        }

        /// <summary>
        /// This method composes and sends the message to a REMO device to set the acquisition mode of a REMO device.
        /// The three possible modes are RAW (About 1 kHz), RAW_IMU (100 Hz) and RMS (About 16 Hz).
        /// </summary>
        /// <param name="device"></param>
        /// <param name="acq"></param>
        /// <returns></returns>
        public static bool SetAcquisitionmode(Device device, ACQmode acq)
        {
            bool ret_value = false;

            ATcommand at_request = new ATcommand(ATcommand.REGISTER.ACQUISITION_MODE, ATcommand.OPERATION.WRITE);
            List<byte> payload = new List<byte>();
            payload.Add((byte)(((byte)acq).ToString().ToCharArray()[0]));

            at_request.ComposeBuffer(payload);
            //  Write command
            device.Serial.Write(at_request.buffer, 0, at_request.buffer.Length);

            //  Read answer
            ATcommand reply = ReadAnswer(device.Serial);
            if (reply != null)
            {
                switch (reply.ack)
                {
                    case ATcommand.ACK.OK:
                        ret_value = true;
                        break;
                    case ATcommand.ACK.ERROR:
                        ret_value = false;
                        break;
                }
            }

            return ret_value;
        }

        public static bool SetOperatingMode(Device device, OpMode op)
        {
            bool ret_value = false;

            ATcommand at_request = new ATcommand(ATcommand.REGISTER.OPERATING_MODE, ATcommand.OPERATION.WRITE);
            List<byte> payload = new List<byte>();
            payload.Add((byte)(((byte)op).ToString().ToCharArray()[0]));

            at_request.ComposeBuffer(payload);

            //  Write command
            device.Serial.Write(at_request.buffer, 0, at_request.buffer.Length);
            device.FirstTimestamp = DateTime.Now;

            //  Read answer
            ATcommand reply = ReadAnswer(device.Serial);
            if (reply != null)
            {
                switch (reply.ack)
                {
                    case ATcommand.ACK.OK:
                        ret_value = true;
                        break;
                    case ATcommand.ACK.ERROR:
                        ret_value = false;
                        break;
                }
            }

            return ret_value;
        }

        public static bool SetMotor(SerialPort com, MotorMode mm)
        {
            bool ret_value = false;

            ATcommand at_request = new ATcommand(ATcommand.REGISTER.MOTOR_MODE, ATcommand.OPERATION.WRITE);
            List<byte> payload = new List<byte>();
            payload.Add((byte)(((byte)mm).ToString().ToCharArray()[0]));

            at_request.ComposeBuffer(payload);

            List<byte> dummy = new List<byte>();
            if (at_request.buffer.Length < 18)
            {
                dummy.AddRange(at_request.buffer);
                for (int i = 0; i < 18 - at_request.buffer.Length; i++)
                    dummy.Add(0);

                at_request.buffer = dummy.ToArray();
            }

            //  Write command
            com.Write(at_request.buffer, 0, at_request.buffer.Length);

            //  Read answer
            ATcommand reply = ReadAnswer(com);
            if (reply != null)
            {
                switch (reply.ack)
                {
                    case ATcommand.ACK.OK:
                        ret_value = true;
                        break;
                    case ATcommand.ACK.ERROR:
                        ret_value = false;
                        break;
                }
            }

            return ret_value;
        }
    }
}

