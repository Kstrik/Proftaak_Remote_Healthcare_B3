using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.VrServer
{
    public class DestWrapper
    {
        public string dest { get; }
        public Message data { get; set; }

        public DestWrapper(string dest)
        {
            this.dest = dest;
            this.data = null;
        }
    }
}
