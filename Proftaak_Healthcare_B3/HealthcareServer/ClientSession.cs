//using Networking;
//using Networking.Client;
//using Networking.Server;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace HealthcareServer
//{
//    public class ClientSession : IServerDataReceiver
//    {
//        // Tuple<ActionId, Action>
//        private List<Tuple<string, Action<JObject>>> receiveQueue;
//        private Client client;

//        private ClientConnection clientConnection;

//        private bool hasSession;
//        private bool hasTunnel;
//        private string sessionId;
//        private string tunnelId;
//        private string tunnelKey;

//        private Scene scene;

//        public ClientSession(ClientConnection clientConnection, ILogger logger)
//        {
//            this.receiveQueue = new List<Tuple<string, Action<JObject>>>();
//            this.client = new Client("145.48.6.10", 6666, this, logger);
//            this.client.Connect();

//            this.clientConnection = clientConnection;

//            this.hasSession = false;
//            this.hasTunnel = false;
//            this.sessionId = "";
//            this.tunnelId = "";
//            this.tunnelKey = "";

//            this.scene = new Scene();
//        }

//        public void OnDataReceived(byte[] data)
//        {
//            JObject jsonData = JObject.Parse(Encoding.UTF8.GetString(data));

//            Tuple<string, Action<JObject>> queuedItem = this.receiveQueue.Where(a => a.Item1 == jsonData.GetValue("id").ToString()).First();
//            this.receiveQueue.Remove(queuedItem);
//            queuedItem.Item2(jsonData);
//        }

//        public void CreateTunnel()
//        {
//            if(!this.hasTunnel)
//            {
//                Thread thread = new Thread(() =>
//                {
//                    RequestCurrentSessions();
//                    while (!this.hasSession) { };
//                    RequestTunnel(this.sessionId);
//                });
//                thread.Start();
//            }
//        }

//        private void RequestCurrentSessions()
//        {
//            byte[] message = Encoding.UTF8.GetBytes("{'id' : 'session/list'}");
//            this.client.Transmit(message);
//            this.receiveQueue.Add(new Tuple<string, Action<JObject>>("session/list", (data) => OnSessionListReceived(data)));
//        }

//        private void RequestTunnel(string sessionId)
//        {
//            byte[] message = Encoding.UTF8.GetBytes("{'id' : 'tunnel/create','data' : {'session' : '" + sessionId + "', 'key' : ''}}");
//            this.client.Transmit(message);
//            this.receiveQueue.Add(new Tuple<string, Action<JObject>>("session/list", (data) => OnTunnelReceived(data)));
//        }

//        private void OnSessionListReceived(JObject jsonData)
//        {

//            this.hasSession = true;
//        }

//        private void OnTunnelReceived(JObject jsonData)
//        {
//            this.tunnelId = ((JObject)jsonData.GetValue("data")).GetValue("id").ToString();
//            this.hasTunnel = true;
//        }
//    }
//}
