using HealthcareClient.BikeConnection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareClient.ServerConnection
{
    /// <summary>
    /// This class keeps track of data from bikes and Heartbeat Monitors. It's purpose is to make sure no data is lost
    /// 
    /// </summary>
    class DataManager
    {
        private ClientMessage clientMessage;
        private IClientMessageReceiver observer;
        

        public DataManager(IClientMessageReceiver observer)
        {
            this.observer = observer;
        }
        public void addPage25(int power, int cadence)
        {
            if (clientMessage.hasPage25)
                pushMessage();
            byte[] powerBytes = BitConverter.GetBytes(power);
            byte[] cadenceBytes = BitConverter.GetBytes(cadence);

            clientMessage.power = new byte[2];
            clientMessage.power[0] = powerBytes[2];
            clientMessage.power[1] = powerBytes[3];
            clientMessage.cadence = cadenceBytes[3];
            clientMessage.hasPage25 = true;
        }
        public void addPage16(int speed)
        {
            if (clientMessage.hasPage16)
                pushMessage();
            byte[] speedBytes = BitConverter.GetBytes(speed);
            clientMessage.speed = speedBytes[3];
            clientMessage.hasPage16 = true;
        }

        public void addHeartbeat(int heartbeat)
        {
            if (clientMessage.hasHeartbeat)
                pushMessage();
            byte[] heartbeatBytes = BitConverter.GetBytes(heartbeat);
            clientMessage.heartbeat = heartbeatBytes[3];
            clientMessage.hasHeartbeat = true;

        }

        private void pushMessage()
        {
            observer.handleClientMessage(clientMessage);
            clientMessage = new ClientMessage();
            clientMessage.hasHeartbeat = false;
            clientMessage.hasPage16 = false;
            clientMessage.hasPage25 = false;
        }

    }
}
