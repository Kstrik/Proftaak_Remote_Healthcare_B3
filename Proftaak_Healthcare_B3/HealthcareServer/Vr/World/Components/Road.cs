﻿using HealthcareServer.Vr.Actions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthcareServer.Vr.World.Components
{
    public class Road : IResponseValidator
    {
        private Session session;
        private Route route;
        public string Id { get; set; }

        private string diffuse;
        private string normal;
        private string specular;
        private float heightOffset;

        public Road(string diffuse, string normal, string specular, float heightOffset, Route route, Session session)
        {
            this.diffuse = diffuse;
            this.normal = normal;
            this.specular = specular;
            this.heightOffset = heightOffset;

            this.route = route;
            this.session = session;
        }

        public async Task Add()
        {
            Response response = await this.session.SendAction(GetAddJsonObject(), new ActionRequest("tunnel/send", "scene/road/add", this));
            this.Id = (response.Status == Response.ResponseStatus.SUCCES) ? (string)response.Value : "";
        }

        public async Task Update()
        {
            await this.session.SendAction(GetUpdateJsonObject());
        }

        private JObject GetAddJsonObject()
        {
            JObject data = new JObject();
            data.Add("route", this.route.Id);
            data.Add("diffuse", this.diffuse);
            data.Add("normal", this.normal);
            data.Add("specular", this.specular);
            data.Add("heightoffset", this.heightOffset);

            JObject roadAdd = new JObject();
            roadAdd.Add("id", "scene/road/add");
            roadAdd.Add("data", data);

            return this.session.GetTunnelSendRequest(roadAdd);
        }

        private JObject GetUpdateJsonObject()
        {
            JObject data = new JObject();
            data.Add("id", this.Id);
            data.Add("route", this.route.Id);
            data.Add("diffuse", this.diffuse);
            data.Add("normal", this.normal);
            data.Add("specular", this.specular);
            data.Add("heightoffset", this.heightOffset);

            JObject roadUpdate = new JObject();
            roadUpdate.Add("id", "scene/road/update");
            roadUpdate.Add("data", data);

            return this.session.GetTunnelSendRequest(roadUpdate);
        }

        public async Task<Response> GetResponse(string jsonResponse)
        {
            string roadId = "";
            string status = "";
            JObject jsonData = ActionRequest.GetActionData(jsonResponse);

            await Task.Run(() =>
            {
                status = jsonData.GetValue("status").ToString();
                if (jsonData.ContainsKey("data"))
                {
                    JObject data = jsonData.GetValue("data").ToObject<JObject>();

                    roadId = data.GetValue("uuid").ToString();
                }
            });

            //Documentatie geeft een verkeerde plaatst aan van het status veld of dit is mogelijk een fout op de VR server

            return new Response((!String.IsNullOrEmpty(roadId) && status.ToLower() == "ok") ? Response.ResponseStatus.SUCCES : Response.ResponseStatus.ERROR, roadId);
        }
    }
}
