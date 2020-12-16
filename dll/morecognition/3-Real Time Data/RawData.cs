namespace morecognition
{
    /// <summary>
    /// This class is a class used to hold all the values coming from the sensor
    /// And to pass these values as event arguments external to the DLL.
    /// </summary>
    public class RawData
    {
        // All the variables involved in data transfer
        public AccelerometerN acc;
        public GyroscopeN gyro;
        public MagnetometerN mag;
        public string Tstamp;
        public float[] emg;


        public RawData()
        {
            this.acc = new AccelerometerN();
            this.gyro = new GyroscopeN();
            this.mag = new MagnetometerN();
            this.emg = new float[8];
            this.Tstamp = string.Empty;
        }
    }

    // The next three classes hold the values relative to the different axis
    public class AccelerometerN
    {
        public float x;
        public float y;
        public float z;
    }

    public class GyroscopeN
    {
        public float x;
        public float y;
        public float z;
    }

    public class MagnetometerN
    {
        public float x;
        public float y;
        public float z;
    }

}
