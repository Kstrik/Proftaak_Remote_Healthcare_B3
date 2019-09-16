using HealthcareServer.Vr.World.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World
{
    public class Scene
    {
        private Session session;

        private List<Node> nodes;
        private List<Route> routes;
        private SkyBox skyBox;

        public Scene(Session session)
        {
            this.nodes = new List<Node>();
            this.routes = new List<Route>();
            this.skyBox = new SkyBox(10, this.session);

            this.session = session;
        }

        public async Task AddNode(Node node)
        {
            if(node != null)
            {
                this.nodes.Add(node);
                await node.Add();
            }
        }

        public async Task AddRoute(Route route)
        {
            if(route != null)
            {
                this.routes.Add(route);
                await route.Add();
            }
        }

        public async Task Reset()
        {
            await this.session.SendAction(GetResetJsonObject());
            this.nodes.Clear();
            this.routes.Clear();
        }

        private JObject GetResetJsonObject()
        {
            JObject resetScene = new JObject();
            resetScene.Add("id", "scene/reset");

            return this.session.GetTunnelSendRequest(resetScene);
        }

        public List<Node> GetNodes()
        {
            return this.nodes;
        }

        public List<Route> GetRoutes()
        {
            return this.routes;
        }

        public SkyBox GetSkyBox()
        {
            return this.skyBox;
        }
    }
}
