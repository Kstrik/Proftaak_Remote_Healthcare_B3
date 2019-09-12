using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;

namespace Networking.VrServer
{
    public class MessageFactory
    {
        string dest;

        //Maak een factiry aan met een dest id. Je moet dus al verbonden zijn met de VR server.
        public MessageFactory(string dest)
        {
            this.dest = dest;
        }

        //Methode om de 2 buitenste lagen aan te maken
        private SendWrapper generateTunnelMessage()
        {
            SendWrapper sendWrapper = new SendWrapper("tunnel/send");
            DestWrapper destWrapper = new DestWrapper(dest);
            sendWrapper.data = destWrapper;
            return sendWrapper;
        }

        //Methode om een bericht in de 2 buitenste lagen te wrappen, klaar voor JSON parsing
        private SendWrapper wrap(Message m)
        {
            SendWrapper sendWrapper = generateTunnelMessage();
            sendWrapper.data.data = m;
            return sendWrapper;
        }

        private string JsonParse(Message m)
        {
            String returnString = JsonConvert.SerializeObject(wrap(m));
            return (returnString);
        }

        public string generateResetMessage()
        {
            Message payload  = new Message("scene/reset");
            return JsonParse(payload);
        }

        public string generateSkyboxTimeMessage(double timeOfDay)
        {
            Message payload = new Message("scene/skybox/settime");
            payload.data.Add("time", timeOfDay);
            return JsonParse(payload);

        }


        //skybox settime
        //roads add
        //terrain add
        //route add
        //node add
        //
        //Todo:
        /*scene
scene/get
scene/reset
scene/save
scene/load
scene/raycast
node
scene/node/add
scene/node/moveto
scene/node/update
scene/node/delete
scene/node/addlayer
scene/node/dellayer
scene/node/find
terrain
scene/terrain/add
scene/terrain/update
scene/terrain/delete
scene/terrain/getheight
panel
scene/panel/clear
scene/panel/drawlines
scene/panel/drawtext
scene/panel/image
scene/panel/setclearcolor
scene/panel/sawp
skybox
scene/skybox/settime
scene/skybox/update
road
scene/road/add
scene/road/update
route
route/add
route/update
route/delete
route/follow
route/follow/speed
route/show
get
setcallback
play
pause*/
    }

    
}
