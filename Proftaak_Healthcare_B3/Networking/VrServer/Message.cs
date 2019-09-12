using System;
using System.Collections.Generic;
using System.Text;

namespace Networking.VrServer
{
    public class Message
    {
        public string id { get; }
        public Dictionary<string, object> data { get; }

        public Message(string id)
        {
            this.id = id;
            this.data = new Dictionary<string, object>();
        }
       
    }
}
