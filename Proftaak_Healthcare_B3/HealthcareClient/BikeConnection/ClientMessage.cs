using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.BikeConnection
{
    struct ClientMessage
    {
        public byte[] power { get; set; }
        public byte cadence { get; set; }

        public byte speed { get; set; }
        public byte heartRate { get; set; }

        public byte checkBits { get; set; }
    }


}
