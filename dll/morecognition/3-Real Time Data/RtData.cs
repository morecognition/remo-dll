using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace morecognition
{
    public class RtData
    {
        #region Properties of the RtData
        // Creating an instance of a Device
        private Device myDevice = new Device();

        // Defining the delegate for the event handler when the data is received
        public event EventHandler<RawData> DataReady;

        // EMG resolution, not sure if this is required or not
        double EMG_resolution = 0.0238; //uV

        // This is for the locking mechanism
        private readonly Object stringLock = new Object();

        internal IList<IRtData> SubscriberList;

        // This is done to assist in the locking of the value assigning statement/process to the lastPacket variable
        String lastPacket;
        public String LastPacket
        {
            get { return lastPacket; }
            set
            {
                lock (stringLock)
                {
                    lastPacket = value;
                }
            }
        }

        #endregion

        #region Mehthods for Acquiring Data


        /// <summary>
        /// This method starts listening to the data coming from the COM port
        /// This is an overload to StartDataAcq method and selects RMS as the
        /// acquisition mode
        /// </summary>
        /// <param name="comName">This is the name of COM Port to listen to</param>
        public void StartDataAcq(string comName)
        {
            // Passing zero to select the acuisition mode as RMS
            StartDataAcq(comName, 0);
        }

        /// <summary>
        /// This method starts listening to the data coming from the COM port
        /// </summary>
        /// <param name="comName">This is the name of COM Port to listen to</param>
        /// <param name="acqMode">This selects the acquisition mode, 0 for RMS, 1 for RAW_IMU</param>
        public async void StartDataAcq(string comName, int acqMode)
        {
            // Assinging myDevice

            //myDevice.ComName = comName;


            myDevice = new Device
            {
                // Assigning the COM port to the device instantiated
                ComName = comName
            };


            // Registering an event handler to the event raised by publisher in myDevice
            myDevice.PacketReceived += MyDevice_PacketReceived;
            

            // Clearing the last packet string
            LastPacket = string.Empty;

            // Starting the data acquisition as a separate task in an asynchronous manner
            
                try
                {
                    switch (acqMode)
                    {
                        // The case where the RMS mode is selected
                        case 0:
                            await myDevice.Start(ATcommand.ACQmode.RMS);
                            break;

                        // The case where the RAW_IMU mode is selected
                        case 1:
                            await myDevice.Start(ATcommand.ACQmode.RAW_IMU);
                            break;

                    // The case where the RAW mode is selected
                        case 2:
                            await myDevice.Start(ATcommand.ACQmode.RAW);
                            break;

                }
                }
                catch
                {
                    return;
                }
        }

        /// <summary>
        /// This method is used to stop the acquisition of data
        /// </summary>
        public async void StopDataAcq()
        {
            await myDevice?.Stop();

            // Calling the stop method in myDevice if myDevice is not null
            if (myDevice != null)
            {
                await myDevice.Stop();
            }
        }



        #endregion

        #region Constructor of class
        public RtData()
        {
             SubscriberList = new List<IRtData>();
        }

        #endregion

        #region Event handler for subscription

        /// <summary>
        /// This is the event handler for whenever the publisher raises the event
        /// In this case the publisher is in Device.cs which bascially raises the
        /// Event when new data is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void MyDevice_PacketReceived(object sender, PacketsArgs e)
        {
            //string temp = string.Empty;

            // making a variable to hold the values extracted in present go
            RawData temp1 = new RawData();

            // Loop to extract sensor readings
            foreach (Sample point in e.packets)
            {

                int counter = 0;
                foreach (byte ch in point.measure.Keys)
                {
                    temp1.emg[counter] = (float)(point.measure[ch] * EMG_resolution);
                    counter++;
                }
                try
                {
                    temp1.acc.x = point.acc.x;
                    temp1.acc.y = point.acc.y;
                    temp1.acc.z = point.acc.z;

                    temp1.gyro.x = point.gyro.x;
                    temp1.gyro.y = point.gyro.y;
                    temp1.gyro.z = point.gyro.z;

                    temp1.mag.x = point.mag.x;
                    temp1.mag.y = point.mag.y;
                    temp1.mag.z = point.mag.z;

                    // Extracting the TimeStamp
                    temp1.Tstamp = point.date.ToString();
                }
                catch
                {

                }
                
            }
            // Invoking subscribers
            OnDataReady(temp1);
        }

        #endregion

        /// <summary>
        /// This is the mehtod which used to raise the events to all subscribers
        /// who have subscribed to DataReady
        /// </summary>
        /// <param name="allData"></param>
        private void OnDataReady(RawData allData)
        {
            DataReady?.Invoke(this, allData);
            foreach (IRtData item in this.SubscriberList)
            {
                item.OnDataReceived(this, new RawData() { acc = allData.acc, emg = allData.emg, gyro = allData.gyro, mag = allData.mag, Tstamp = allData.Tstamp });
            }
        }

        #region Aggiungi e Rimuovi Subscribers
        /// <summary>
        /// Metodo di aggiunta subscriber
        /// </summary>
        /// <param name="Subscriber"></param>
        public void AddSubscriber(IRtData Subscriber)
        {
            this.SubscriberList.Add(Subscriber);
        }

        /// <summary>
        /// Metodo di rimozione Subscriber
        /// </summary>
        /// <param name="Subscriber"></param>
        public void RemoveSubscriber(IRtData Subscriber)
        {
            this.SubscriberList.Remove(Subscriber);
        }

    }
}
#endregion