using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.VrServer
{
    public class SendWrapper
    {
        public string id { get; }
        public DestWrapper data { get; set; }

        public SendWrapper(string id)
        {
            this.id = id;
            this.data = null;
        }
    }
}
