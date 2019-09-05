using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatieTest
{
    class RealBike : Bike
    {
        public RealBike(IBikeDataReceiver bikeDataReceiver) 
            : base(bikeDataReceiver)
        {

        }

        public override void ReceivedData(byte[] data)
        {
            base.ReceivedData(data);
        }

        public override bool ToggleListening()
        {
            throw new NotImplementedException();
        }

        public override bool StartListening()
        {
            throw new NotImplementedException();
        }

        public override bool StopListening()
        {
            throw new NotImplementedException();
        }
    }
}
