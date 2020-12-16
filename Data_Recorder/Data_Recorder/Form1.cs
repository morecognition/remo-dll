using morecognition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data_Recorder
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Device to be connected
        /// </summary>
        public static RtData myDevice = new RtData();

        /// <summary>
        /// Flag to see if the exercise has started or not and thus data must be stored or not
        /// </summary>
        public static bool recordData { get; set; }

        /// <summary>
        /// Flag to check if the device is connected or not
        /// </summary>
        public static bool deviceConnected { get; set; }

        /// <summary>
        /// The object that will contain all the data collected in a session
        /// </summary>
        public List<SensorData> allData { get; set; }

        /// <summary>
        /// The default path where the application is running
        /// </summary>
        public static string defaultPath { get; set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Disabling buttons that are not being used
            disconnectButton.Enabled = false;
            startRecordingDataButton.Enabled = false;
            stopRecordingDataButton.Enabled = false;

            defaultPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Recorded Data\\";

            allData = new List<SensorData>();

            //counterText.TextAlign = ContentAlignment.MiddleLeft;
            //phaseNameText.TextAlign = ContentAlignment.MiddleLeft;

        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            // Checking if the COM box or the user name box is empty
            if (comPortTextBox.Text == "")
            {
                MessageBox.Show("Please fill in the COM port number and retry.");
                return;
            }

            // Getting the value written in the COM port box
            var portNumber = "COM" + comPortTextBox.Text;

            // Clearing the text box
            comPortTextBox.Text = "";

            // Starting communication with the device
            myDevice.StartDataAcq(portNumber);

            // Subscribing to event
            myDevice.DataReady += StoreData;

            

            // For testing only
            //connectionString.Text = portNumber;
        }

        /// <summary>
        /// Function that will be called each time the packet is recieved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StoreData(object sender, RawData e)
        {
            // First time a connection is made different components of the form are changed
            if (!deviceConnected)
            {
                deviceConnected = true;
                this.Invoke((MethodInvoker)(() =>
                {

                    // Changing connection status string
                    connectionString.Text = "Connected";
                    connectionString.ForeColor = Color.Green;

                    // Disabling connect button
                    connectButton.Enabled = false;

                    // Enabling other buttons
                    disconnectButton.Enabled = true;
                    startRecordingDataButton.Enabled = true;
                    stopRecordingDataButton.Enabled = true;

                }));
            }

            if (recordData)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    var DataText = "Accx: " + e.acc.x.ToString() + "\n";
                    DataText += "Accy: " + e.acc.y.ToString() + "\n";
                    DataText += "Accz: " + e.acc.z.ToString() + "\n";
                    DataText += "Gyrox: " + e.gyro.x.ToString() + "\n";
                    DataText += "Gyroy: " + e.gyro.y.ToString() + "\n";
                    DataText += "Gyroz: " + e.gyro.z.ToString() + "\n";
                    DataText += "Magx: " + e.mag.x.ToString() + "\n";
                    DataText += "Magy: " + e.mag.y.ToString() + "\n";
                    DataText += "Magz: " + e.mag.z.ToString() + "\n";
                    DataText += "EMG1: " + e.emg[0].ToString() + "\n";
                    DataText += "EMG2: " + e.emg[1].ToString() + "\n";
                    DataText += "EMG3: " + e.emg[2].ToString() + "\n";
                    DataText += "EMG4: " + e.emg[3].ToString() + "\n";
                    DataText += "EMG5: " + e.emg[4].ToString() + "\n";
                    DataText += "EMG6: " + e.emg[5].ToString() + "\n";
                    DataText += "EMG7: " + e.emg[6].ToString() + "\n";
                    DataText += "EMG8: " + e.emg[7].ToString() + "\n";
                    Data.Text = DataText;
                }));

                
                allData.Add(new SensorData
                {
                    TimeStamp = e.Tstamp.ToString(),
                    userName = "User 1",
                    Phase = 0,
                    accx = e.acc.x,
                    accy = e.acc.y,
                    accz = e.acc.z,
                    gyrox = e.gyro.x,
                    gyroy = e.gyro.y,
                    gyroz = e.gyro.z,
                    magx = e.mag.x,
                    magy = e.mag.y,
                    magz = e.mag.z,
                    emg = e.emg,
                }); ;

                
                
            }

        }

        private async void startRecordingDataButton_Click(object sender, EventArgs e)
        {
            // Toggling the flag to store data to true
            recordData = true;

            startRecordingDataButton.Enabled = false;

        }

        private void stopRecordingDataButton_Click(object sender, EventArgs e)
        {
            // Toggling the flag to store data to fakse
            recordData = false;
            startRecordingDataButton.Enabled = true;

            var presentTime = new DateTime();
            presentTime = DateTime.Now;

            var filenameJSON =
                        presentTime.Hour.ToString().PadLeft(2, '0') +
                        presentTime.Minute.ToString().PadLeft(2, '0') + "_" +
                        presentTime.Day.ToString().PadLeft(2, '0') +
                        presentTime.Month.ToString().PadLeft(2, '0') +
                        presentTime.Year.ToString() +
                        ".json";


            var jsonObject = JsonConvert.SerializeObject(allData);

            if (Directory.Exists(defaultPath))
            {
                File.WriteAllText(defaultPath + filenameJSON, jsonObject);
            }
            else
            {
                Directory.CreateDirectory(defaultPath);
                File.WriteAllText(defaultPath + filenameJSON, jsonObject);
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            

            // Changing connection status string
            connectionString.Text = "Disconnected";
            connectionString.ForeColor = Color.Red;


            // Enabling other buttons
            disconnectButton.Enabled = false;
            startRecordingDataButton.Enabled = false;
            stopRecordingDataButton.Enabled = false;

            // Removing the subscriber
            myDevice.DataReady -= StoreData;

            // Stopping device connection
            myDevice.StopDataAcq();
        }
    }

    #region Object Defintion for SensorData Class
    public class SensorData
    {
        #region Properties

        public string TimeStamp { get; set; }

        public int Phase { get; set; }

        public float accx { get; set; }

        public float accy { get; set; }

        public float accz { get; set; }

        public float gyrox { get; set; }

        public float gyroy { get; set; }

        public float gyroz { get; set; }

        public float magx { get; set; }

        public float magy { get; set; }

        public float magz { get; set; }

        public float[] emg { get; set; }

        public string userName { get; set; }

        #endregion


        #region Constructor
        public SensorData()
        {
            TimeStamp = string.Empty;
            Phase = new int();
            accx = new float();
            accy = new float();
            accz = new float();
            gyrox = new float();
            gyroy = new float();
            gyroz = new float();
            magx = new float();
            magy = new float();
            magz = new float();
            emg = new float[8];
            userName = string.Empty;
        }


        #endregion

    }
    #endregion
}
