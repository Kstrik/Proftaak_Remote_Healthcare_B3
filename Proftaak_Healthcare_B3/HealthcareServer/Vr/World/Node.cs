using HealthcareServer.Vr.Actions;
using HealthcareServer.Vr.VectorMath;
using HealthcareServer.Vr.World.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World
{
    public class Node : IResponseValidator
    {
        private Session session;

        public string Id { get; set; }
        public string Name { get; }
        public string ParentId { get; }

        private Transform transform;
        private Model model;
        private Terrain terrain;
        private Panel panel;

        public Node(string name, Session session, string parentId = "")
        {
            this.Name = name;
            this.ParentId = parentId;
            this.session = session;
        }

        public void SetTransform(Transform transform)
        {
            if (transform != null)
                this.transform = transform;
        }

        public void SetModel(Model model)
        {
            if (model != null)
                this.model = model;
        }

        public void SetTerrain(Terrain terrain)
        {
            if (terrain != null)
                this.terrain = terrain;
        }

        public void SetPanel(Panel panel)
        {
            if (panel != null)
                this.panel = panel;
        }

        public async Task Add()
        {
            if(this.terrain != null)
                await Task.Run(() => this.terrain.Add());

            Response response = await this.session.SendAction(GetAddJsonObject(), new ActionRequest("tunnel/send", "scene/node/add", this));
            this.Id = (response.Status == Response.ResponseStatus.SUCCES) ? (string)response.Value : "";
        }

        public async Task Update()
        {
            await this.session.SendAction(GetUpdateJsonObject());
        }

        public async Task FollowRoute(Route route, float speed, Vector3 positionOffset, Vector3 rotateOffset, bool followHeight = false, float offset = 0.0f, float smoothing = 1.0f)
        {
            await this.session.SendAction(GetFollowRouteJsonObject(route, speed, positionOffset, rotateOffset, followHeight, offset, smoothing));
        }

        private JObject GetAddJsonObject()
        {
            JObject components = new JObject();
            if (this.transform != null)
                components.Add("transform", this.transform.GetJsonObject());
            if (this.model != null)
                components.Add("model", this.model.GetJsonObject());
            if(this.terrain != null)
            {
                JObject jsonTerrain = new JObject();
                jsonTerrain.Add("smoothnormals", this.terrain.SmoothNormals);

                components.Add("terrain", jsonTerrain);
            }

            JObject data = new JObject();
            data.Add("name", this.Name);
            if (!String.IsNullOrEmpty(this.ParentId))
                data.Add("parent", this.ParentId);
            data.Add("components", components);

            JObject nodeAdd = new JObject();
            nodeAdd.Add("id", "scene/node/add");
            nodeAdd.Add("data", data);

            return this.session.GetTunnelSendRequest(nodeAdd);
        }

        private JObject GetUpdateJsonObject()
        {
            JObject data = new JObject();
            data.Add("name", this.Name);
            if (!String.IsNullOrEmpty(this.ParentId))
                data.Add("parent", this.ParentId);
            if (this.transform != null)
                data.Add("transform", this.transform.GetJsonObject());
            if (this.model != null)
                data.Add("model", this.model.GetJsonObject());
            if (this.terrain != null)
            {
                JObject jsonTerrain = new JObject();
                jsonTerrain.Add("smoothnormals", this.terrain.SmoothNormals);

                data.Add("terrain", jsonTerrain);
            }

            JObject nodeUpdate = new JObject();
            nodeUpdate.Add("id", "scene/node/update");
            nodeUpdate.Add("data", data);

            return this.session.GetTunnelSendRequest(nodeUpdate);
        }

        private JObject GetFollowRouteJsonObject(Route route, float speed, Vector3 positionOffset, Vector3 rotateOffset, bool followHeight = false, float offset = 0.0f, float smoothing = 1.0f)
        {
            JObject data = new JObject();
            data.Add("route", route.Id);
            data.Add("node", this.Id);
            data.Add("speed", speed);
            data.Add("offset", offset);
            data.Add("rotate", "XZ");
            data.Add("smoothing", smoothing);
            data.Add("followHeight", followHeight);
            data.Add("rotateOffset", rotateOffset.GetJsonObject());
            data.Add("positionOffset", positionOffset.GetJsonObject());

            JObject followRoute = new JObject();
            followRoute.Add("id", "route/follow");
            followRoute.Add("data", data);

            return this.session.GetTunnelSendRequest(followRoute);
        }

        public async Task<Response> GetResponse(string jsonResponse)
        {
            string nodeId = "";
            string status = "";
            JObject jsonData = ActionRequest.GetActionData(jsonResponse);

            await Task.Run(() =>
            {
                status = jsonData.GetValue("status").ToString();
                if (jsonData.ContainsKey("data"))
                {
                    JObject data = jsonData.GetValue("data").ToObject<JObject>();

                    nodeId = data.GetValue("uuid").ToString();
                }
            });

            return new Response((!String.IsNullOrEmpty(nodeId) && status.ToLower() == "ok") ? Response.ResponseStatus.SUCCES : Response.ResponseStatus.ERROR, nodeId);
        }
    }
}
