using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Recorder
{
    /// <summary>
    /// This class will store sensor data and will be converted to JSON format
    /// </summary>
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
        }


        #endregion

    }
}
