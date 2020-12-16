using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace morecognition
{
    internal class Sample
    {
        public DateTime date;
        public Dictionary<byte, Int32> measure;
        public UInt32 total_decimilliseconds;
        public Accelerometer acc;
        public Gyroscope gyro;
        public Magnetometer mag;

        public Sample()
        {
            this.measure = new Dictionary<byte, Int32>();
            this.acc = new Accelerometer();
            this.gyro = new Gyroscope();
            this.mag = new Magnetometer();
        }
    }

    internal class Accelerometer : IMU
    { }

    internal class Gyroscope : IMU
    { }

    internal class Magnetometer : IMU
    { }

    internal class IMU
    {
        public float x = 0.0f;
        public float y = 0.0f;
        public float z = 0.0f;
    }

    internal class PacketsArgs : EventArgs
    {
        public List<Sample> packets;

        public PacketsArgs(List<Sample> p_list)
        {
            packets = new List<Sample>(p_list);
        }
    }
}
