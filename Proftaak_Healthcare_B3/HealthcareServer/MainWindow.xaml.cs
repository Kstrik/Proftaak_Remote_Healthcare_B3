using HealthcareServer.Vr;
using HealthcareServer.Vr.VectorMath;
using HealthcareServer.Vr.World;
using HealthcareServer.Vr.World.Components;
using Networking;
using Networking.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HealthcareServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServerDataReceiver, ILogger
    {
        private Client client;

        private Session session;
        private SkyBox skyBox;
        private Terrain terrain;
        private Node node;
        private Route route;
        private Road road;

        private bool isInitialized;
        private bool terrainAdded;

        public MainWindow()
        {
            InitializeComponent();

            //Transform transform = new Transform(new Vector3(0, 0, 0), 1.0f, new Vector3(0, 0, 0));
            //JObject jsonObject = transform.GetJsonObject().ToObject<JObject>();

            //string test = jsonObject.ToString();

            //SkyBox skyBox = new SkyBox(12);
            //skyBox.Time = 19;

            this.client = new Client("145.48.6.10", 6666, this, this);
            this.session = new Session(ref client, "DESKTOP-KENLEY");
            this.client.Connect();
            this.isInitialized = false;
            this.terrainAdded = false;
        }

        private async Task Initialize()
        {
            await session.Create();
            this.skyBox = new SkyBox(12, this.session);
            this.isInitialized = true;
        }

        public void Log(string text)
        {

        }

        public void OnDataReceived(byte[] data)
        {
            this.session.OnDataReceived(data);
        }

        private async void SetTime_Click(object sender, RoutedEventArgs e)
        {
            this.route = new Route(this.session);
            this.route.AddRouteNode(new Route.RouteNode(new Vector3(5, 0, 0), new Vector3(1.0f, 0, 0)));
            this.route.AddRouteNode(new Route.RouteNode(new Vector3(50, 0, 0), new Vector3(0.5f, 0, 0)));
            await this.route.Add();

            this.road = new Road("data/NetworkEngine/textures/tarmac_diffuse.png", "data/NetworkEngine/textures/tarmac_normale.png", "data/NetworkEngine/textures/tarmac_specular.png", 0.01f, this.route, this.session);
            await this.road.Add();

            await this.node.FollowRoute(this.route, 5.0f, new Vector3(0, 0, 0), new Vector3(0, 0, 0));

            //await this.skyBox.SetTime(int.Parse(txb_Time.Text));
        }

        private async void SessionStart_Click(object sender, RoutedEventArgs e)
        {
            await Initialize();
        }

        private async void AddTerrain_Click(object sender, RoutedEventArgs e)
        {
            //if(!this.terrainAdded)
            //{
                this.terrain = new Terrain(int.Parse(txb_TerrainWidth.Text), int.Parse(txb_TerrainDepth.Text), int.Parse(txb_MaxHeight.Text), @"C:\Users\Kenley Strik\Documents\School\Leerjaar 2019-2020\Periode 1\Proftaak Remote Healthcare\Sprint 2\HeightMap.jpeg", true, this.session);
                await this.terrain.Add();
                this.terrainAdded = true;
            //}
        }

        private async void UpdateTerrain_Click(object sender, RoutedEventArgs e)
        {
            if (this.terrainAdded)
            {
                this.terrain.SetMaxHeight(int.Parse(txb_MaxHeight.Text));
                await this.terrain.Update();
            }
        }

        private async void DeleteTerrain_Click(object sender, RoutedEventArgs e)
        {
            if (this.terrainAdded)
            {
                await this.terrain.Delete();
            }
        }

        private async void AddNode_Click(object sender, RoutedEventArgs e)
        {
            this.node = new Node(txb_NodeName.Text, this.session);

            Transform transform = new Transform(new Vector3(0, 0, 0), 1.0f, new Vector3(0, 0, 0));
            this.node.SetTransform(transform);
            this.node.SetTerrain(this.terrain);

            Model model = new Model("data/NetworkEngine/models/trees/fantasy/tree1.obj", false);
            this.node.SetModel(model);

            await this.node.Add();
        }

        private async void UpdateNode_Click(object sender, RoutedEventArgs e)
        {
            await this.node.Update();
        }

        private async void ResetScene_Click(object sender, RoutedEventArgs e)
        {
            await this.session.GetScene().Reset();
        }
    }
}
