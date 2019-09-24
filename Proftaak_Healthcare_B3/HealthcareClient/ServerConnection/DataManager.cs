﻿#define DEBUG 

using HealthcareClient.Bike;
using HealthcareClient.BikeConnection;
using Networking.Client;
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
    class DataManager: IServerDataReceiver, IBikeDataReceiver, IClientMessageReceiver
    {
        private ClientMessage clientMessage;
        private Client DataServerClient;

        private IClientMessageReceiver observer;

        [Flags] public enum CheckBits { Sessie = 0b0001000, BikeError = 0b0000100, HeartBeatError = 0b00000010, VRError = 0b00000001 };


        public DataManager(IClientMessageReceiver observer) //current observer is datamanager itself, rather than the client window
        {
            this.observer = this;
            DataServerClient = new Client("localhost", 80, this, null);

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
#if (DEBUG)
            Console.WriteLine("Pushing message");
#endif
            observer.handleClientMessage(clientMessage);
            clientMessage = new ClientMessage();
            clientMessage.hasHeartbeat = false;
            clientMessage.hasPage16 = false;
            clientMessage.hasPage25 = false;
        }

        //Upon receiving data from the bike and Heartbeat Sensor, try to place in a Struct. 
        //Once struct is full or data would be overwritten, it is sent to the server
        void IBikeDataReceiver.ReceiveBikeData(byte[] data, Bike.Bike bike)
        {
            Dictionary<string, int> translatedData = TacxTranslator.Translate(BitConverter.ToString(data).Split('-'));
            int PageID;
            translatedData.TryGetValue("PageID", out PageID); //hier moet ik van overgeven maar het kan niet anders
            if (25 == PageID)
            {
                int power; translatedData.TryGetValue("InstantaneousPower", out power);
                int cadence; translatedData.TryGetValue("InstantaneousCadence", out cadence);
                addPage25(power, cadence);
            }
            else if (16 == PageID)
            {
                int speed; translatedData.TryGetValue("speed", out speed);
                addPage16(speed);
            }
        }

        private void ReceiveHeartbeatData(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses a complete ClientMessage into a packet to be sent via TCP
        /// </summary>
        void IClientMessageReceiver.handleClientMessage(ClientMessage clientMessage)
        {
            byte clientID = 0b00000001; // message is from a client
            byte Checkbits = (byte)CheckBits.HeartBeatError; //heartbeat not implemented yet
            byte[] message = clientMessage.toByteArray();
            message.Prepend(clientID);
            message.Append(Checkbits);
            Send(message);
        }

        private void Send(byte[] message)
        {
            DataServerClient.Transmit(message);        }

        public void OnDataReceived(byte[] data)
        {
#if DEBUG
            Console.WriteLine("Received response from data server - response not handled");
#endif
        }

       
    }
}
