using System;
using Networking;
using Networking.VrServer;
using Newtonsoft.Json;

namespace MessageFactoryTest
{
    class Program
    {
        static void Main(string[] args)
        {
                MessageFactory messageFactory = new MessageFactory("testDestination");
                string jsonSTring = messageFactory.generateResetMessage();
                Console.WriteLine(jsonSTring);
                Console.WriteLine("Press a key to start reverse parsing from simulated object");
                Console.ReadKey();
                SendWrapper sw = JsonConvert.DeserializeObject<SendWrapper>(jsonSTring);
            Console.WriteLine(sw.id);
            Console.WriteLine(sw.data);
            Console.ReadKey();

            string jsonString2 = @"{
                'id' : 'tunnel/send',
	'data' :
	{
                    'id' : '2150373e-d796-4e3d-a2b7-c50be14feaa1',
		'data' : 
		{
                        'id' : 'scene/reset',
			'status' : 'ok'

        }
                }
            }";
            SendWrapper sw2 = JsonConvert.DeserializeObject<SendWrapper>(jsonString2);
            Console.WriteLine(sw2.id);
            Console.WriteLine(sw2.data);
            Console.ReadKey();



        }
    }
    
}
