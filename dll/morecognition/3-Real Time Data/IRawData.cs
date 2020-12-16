using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace morecognition
{
    public interface IRtData
    {
        void OnDataReceived(object source, RawData e);
    }
}
