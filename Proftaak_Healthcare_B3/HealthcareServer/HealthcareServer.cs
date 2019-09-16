using Networking;
using Networking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer
{
    public class HealthCareServer : IServerConnector, IClientDataReceiver
    {
        private Server server;
        //private List<ClientSession> sessions;

        public HealthCareServer()
        {
            
        }

        public void OnClientConnected(ClientConnection connection)
        {
            //Session session = new Session();
            //this.sessions.Add(session);
        }

        public void OnClientDisconnected(ClientConnection connection)
        {
            throw new NotImplementedException();
        }

        public void OnDataReceived(byte[] data, string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
