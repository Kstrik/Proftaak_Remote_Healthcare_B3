using System;
using System.Collections.Generic;
using System.Text;

namespace Networking
{
    public interface IDataReceiver
    {
        void OnDataReceived(byte[] data);
    }
}
