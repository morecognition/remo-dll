using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace morecognition
{
    internal class ATcommand
    {
        /// <summary>
        /// This enum defines the carriage return and Line feed bytes that need to be sent at the end of the message.
        /// These are bytes 7 and 8 of a total of 8 bytes.
        /// </summary>
        public enum CR_LF_BYTE
        {
            AT_COMMAND_CR = 13,     //  Carriage Return 0x0D STOP = <CR> + <LF> = '13' + '10'
            AT_COMMAND_LF = 10,     //  Line Feed 0x0A
        }

        /// <summary>
        /// This enum defines the different modes the register byte can take.
        /// This is the 4th byte out of a total 8 bytes sent in the message.
        /// The value of this enumerator must be converted to a string before being converted to a raw byte.
        /// </summary>
        public enum REGISTER
        {
            ACQUISITION_MODE = 1,
            OPERATING_MODE = 2,
            MOTOR_MODE = 3
        }

        /// <summary>
        /// This enum defines the different types of data that can be received from REMO.
        /// This is the 6th out of the 8 bytes sent. 
        /// The value of this enumerator must be converted to a string before being converted to a raw byte.
        /// </summary>
        public enum ACQmode
        {
            RAW = 1,
            RMS = 2,
            RAW_IMU = 3
        }

        /// <summary>
        /// This enum is used to set the (possibly) operating mode of the serail port.
        /// Values need to be converted to string before being converted to a byte.
        /// It is sent in the "setOperatingMode" function and is the 6th out of 8 bytes.
        /// </summary>
        public enum OpMode
        {
            ClientConnected = 1,
            TxMode = 2
        }

        /// <summary>
        /// This is a byte used to enable or disable a MOTOR in-built into a REMO device.
        /// </summary>
        public enum MotorMode
        {
            Enabled = 1,
            Disabled = 2
        }

        /// <summary>
        /// OPERATION enumerator defines the 4th byte of the message sent. Total of 8 bytes.
        /// </summary>
        public enum OPERATION
        {
            WRITE = '=',
            READ = '?',
            HELP = '&'
        }

        /// <summary>
        /// ACK enumerator defines and maps responses received from the REMO device.
        /// </summary>
        public enum ACK
        {
            OK = 'O',
            ERROR = 'E',
        }

        // Sets the different modes of operation of a REMO device
        public REGISTER reg;

        // Sets the type of an AT command
        public OPERATION op;

        // This byte is used in place of the byte which sets the acquisition mode of the device.
        // Also ussed in the processing of the response received from a REMO device.
        public UInt16 value;

        // Unknown
        public byte value2;

        // Used to map responses from a REMO device after receiving a command
        public ACK ack;

        // Buffer that holds the bytes to be sent to a REMO device via SPP or the reply received from a device after a message was sent.
        public byte[] buffer;

        /// <summary>
        /// Constructor of the ATcommand class
        /// </summary>
        public ATcommand()
        {

        }

        /// <summary>
        /// Constructor of the ATcommand class.
        /// </summary>
        /// <param name="registerNum">Sets the operational mode of REMO (Operating, acquistion or motor).</param>
        /// <param name="oper">Sets the type of message sent to the REMO device.</param>
        public ATcommand(REGISTER registerNum, OPERATION oper)
        {
            this.reg = registerNum;
            this.op = oper;
            this.value = 0;
        }

        /// <summary>
        /// Constructor of the ATcommand class.
        /// </summary>
        /// <param name="registerNum">Sets the operation mode of REMO.</param>
        /// <param name="oper">Sets the type of message sent to the REMO device (Write, Read or Help).</param>
        /// <param name="val">This value is used in the place of the byte that sets the acquisition mode of the REMO device.</param>
        public ATcommand(REGISTER registerNum, OPERATION oper, UInt16 val)
        {
            this.reg = registerNum;
            this.op = oper;
            this.value = val;
        }

        /// <summary>
        /// This method composes the buffer which is sent to REMO carrying the commands.
        /// A total of 8 Bytes are sent in the message to the REMO device.
        /// </summary>
        public void ComposeBuffer()
        {
            // List of data (type byte) that will then be converted into an array of bytes
            List<byte> data = new List<byte>();

            // BYTE 1: A
            data.Add((byte)'A');

            // BYTE 2: T
            data.Add((byte)'T');

            // BYTE 3: S
            data.Add((byte)'S');

            // BYTE 4: Byte which sends the acquistion mode
            data.AddRange(Encoding.ASCII.GetBytes(((byte)reg).ToString()));

            if (op == OPERATION.WRITE)
            {
                // BYTE 5: Byte that defines the type of operation (Write, Read or Help)
                data.Add((byte)op);

                // BYTE 6: Byte that defines the mode of acquisition (RAW, RAW_IMU or RMS)
                data.AddRange(Encoding.ASCII.GetBytes(value.ToString()));
            }
            else if (op == OPERATION.READ)
            {
                // BYTE 5: Byte that defines the type of operation (Write, Read or Help)
                data.Add((byte)op);
                if (value != 0)
                {
                    // BYTE 6: Byte that defines the mode of acquisition (RAW, RAW_IMU or RMS)
                    data.AddRange(Encoding.ASCII.GetBytes(value.ToString()));
                }
            }
            else if (op == OPERATION.HELP)
            {
                // BYTE 5: Byte that defines the type of operation (Write, Read or Help)
                data.Add((byte)op);

                // BYTE 6: Byte that defines the mode of acquisition (RAW, RAW_IMU or RMS)
                data.AddRange(Encoding.ASCII.GetBytes(value.ToString()));
            }

            // BYTE 7: Carriage return byte.
            data.Add((byte)CR_LF_BYTE.AT_COMMAND_CR);

            // BYTE 8: Line feed byte.
            data.Add((byte)CR_LF_BYTE.AT_COMMAND_LF);

            // The composed data is written to the buffer property of the class
            buffer = data.ToArray();
        }

        /// <summary>
        /// Compose buffer which takes as an argument the type of data acquisition.
        /// </summary>
        /// <param name="value_list">This parameter holds the type of acquisition.</param>
        public void ComposeBuffer(List<byte> value_list)
        {
            // For detailed explanation of each element in the data list please refer to the previous method ComposeBuffer().
            // The only difference is that the acquistion mode (BYTE 6) is not an input parameter an is not set using the class
            // proerty "value".
            List<byte> data = new List<byte>();

            data.Add((byte)'A');
            data.Add((byte)'T');
            data.Add((byte)'S');
            data.AddRange(Encoding.ASCII.GetBytes(((byte)reg).ToString()));

            if (op == OPERATION.WRITE)
            {
                data.Add((byte)op);
                data.AddRange(value_list);
            }
            else if (op == OPERATION.READ)
            {
                data.Add((byte)op);
                if (value != 0)
                {
                    data.AddRange(value_list);
                }
            }
            else if (op == OPERATION.HELP)
            {
                data.Add((byte)op);
                data.AddRange(value_list);
            }

            data.Add((byte)CR_LF_BYTE.AT_COMMAND_CR);
            data.Add((byte)CR_LF_BYTE.AT_COMMAND_LF);

            buffer = data.ToArray();
        }

        /// <summary>
        /// Method that processes the reply received from a REMO device after a message is sent.
        /// </summary>
        /// <param name="reply">A list containing all the bytes in the reply.</param>
        /// <returns></returns>
        public bool ProcessReply(List<byte> reply)
        {
            bool ret_value = false;

            // Converting the reply list into an array named buffer not used in this method.
            buffer = reply.ToArray();

            // Checking the length of the reply list, if it is greater than 3
            if (reply.Count > 3)
            {
                // First byte is the acknowledgement byte E or O.
                ack = (ACK)reply[0];

                // Acknowledgement parsed hence removing it and the next two bytes (the next two bytes carry no information)
                reply.RemoveAt(0);
                reply.RemoveAt(reply.Count - 1);
                reply.RemoveAt(reply.Count - 1);

                value = 0;  

                bool catched = false;

                // Trying to convert the rest into an unsigned integer and and setting the 'value' property
                try
                {
                    value = UInt16.Parse(Encoding.ASCII.GetString(reply.ToArray()));
                }
                catch (Exception)
                {
                    catched = true;
                }

                if (!catched)
                {
                    ret_value = true;
                }
                else
                {
                    // If unable to convert the rest of the message to UInt16, converting it to a string and setting 'value' property.
                    if (Encoding.ASCII.GetString(reply.ToArray()) == "yes")
                    {
                        value = 1;
                        ret_value = true;
                    }
                    else
                    {
                        if (Encoding.ASCII.GetString(reply.ToArray()) == "no")
                        {
                            value = 0;
                            ret_value = true;
                        }
                    }
                }
            }
            else if (reply[0] == 'O' || reply[0] == 'E')
            {
                // If length of reply is less than or equal to three and if the last to bytes are carriage return and line feed
                // setting 'ack' property.
                ack = (ACK)reply[0];

                if (reply[1] == (byte)CR_LF_BYTE.AT_COMMAND_CR && reply[2] == (byte)CR_LF_BYTE.AT_COMMAND_LF)
                {
                    ret_value = true;
                }
            }

            return ret_value;
        }
    }
}
