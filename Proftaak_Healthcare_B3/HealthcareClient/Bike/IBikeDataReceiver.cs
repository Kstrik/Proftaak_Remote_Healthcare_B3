using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatieTest
{
    public interface IBikeDataReceiver
    {
        void ReceiveBikeData(byte[] data, Bike bike);
    }
}
