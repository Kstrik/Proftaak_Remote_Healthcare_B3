using HealthcareServer.Vr;
using HealthcareServer.Vr.VectorMath;
using HealthcareServer.Vr.World;
using HealthcareServer.Vr.World.Components;
using Networking;
using Networking.Client;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using UIControls;
using UIControls.Menu;

namespace HealthcareServer
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window, IServerDataReceiver, ILogger
    {
        private Client client;
        private Session session;

        private SidemenuWindow sidemenuWindow;
        private Grid grid;

        private Container sceneContainer;
        private Container propertiesContainer;
        private Container addContainer;
        private Container logContainer;

        private TreeView treeView;
        private TreeViewItem skyBox;
        private TreeViewItem nodes;
        private TreeViewItem routes;
        private TreeViewItem scene;

        private TextBox logField;

        public Dashboard()
        {
            InitializeComponent();

            this.sidemenuWindow = new SidemenuWindow(new Sidemenu(250, 50, 400, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007acc")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))));

            grd_Grid.Children.Add(sidemenuWindow);
            InitializeUIComponents();

            this.Closing += Dashboard_Closing;

            //LOG
            this.logField = new TextBox();
            this.logField.AcceptsReturn = true;
            this.logField.Margin = new Thickness(0, 0, 0, 0);
            this.logField.Foreground = Brushes.White;
            this.logField.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            this.logField.BorderBrush = Brushes.Transparent;
            this.logField.Height = 250;
            this.logContainer.GetContentPanel().Children.Add(this.logField);

            this.client = new Client("145.48.6.10", 6666, this, this);
            this.session = new Session(ref client, "DESKTOP-KENLEY");
            this.client.Connect();

            Button resetSceneButton = new Button();
            resetSceneButton.Content = "Reset scene";
            resetSceneButton.Foreground = Brushes.White;
            resetSceneButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            resetSceneButton.BorderBrush = Brushes.Transparent;
            resetSceneButton.Margin = new Thickness(5, 5, 5, 5);
            resetSceneButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) => 
            {
                Task.Run(() => this.session.GetScene().Reset());
            });
            Button startSessionButton = new Button();
            startSessionButton.Content = "Start session";
            startSessionButton.Foreground = Brushes.White;
            startSessionButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            startSessionButton.BorderBrush = Brushes.Transparent;
            startSessionButton.Margin = new Thickness(5, 5, 5, 5);
            startSessionButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                Task.Run(() => Initialize());
            });

            this.propertiesContainer.GetContentPanel().Children.Add(resetSceneButton);
            this.propertiesContainer.GetContentPanel().Children.Add(startSessionButton);
        }

        private void Dashboard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private async Task Initialize()
        {
            await this.session.Create();
        }

        private void InitializeUIComponents()
        {
            SetupGrid();
            SetupContainers();
            SetupTreeView();
            SetupTabControl();
        }

        private void SetupGrid()
        {
            this.grid = new Grid();
            this.grid.Margin = new Thickness(2, 2, 2, 2);
            //grid.ShowGridLines = true;
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            RowDefinition r1 = new RowDefinition();
            RowDefinition r2 = new RowDefinition();
            RowDefinition r3 = new RowDefinition();
            c1.Width = new GridLength(10, GridUnitType.Star);
            c2.Width = new GridLength(20, GridUnitType.Star);
            r1.Height = new GridLength(200, GridUnitType.Star);
            r2.Height = new GridLength(200, GridUnitType.Star);
            r3.Height = new GridLength(200, GridUnitType.Star);
            this.grid.ColumnDefinitions.Add(c1);
            this.grid.ColumnDefinitions.Add(c2);
            this.grid.RowDefinitions.Add(r1);
            this.grid.RowDefinitions.Add(r2);
            this.grid.RowDefinitions.Add(r3);

            this.sidemenuWindow.GetContentPanel().Children.Add(this.grid);
        }

        private void SetupContainers()
        {
            this.sceneContainer = new Container("Scene");
            this.propertiesContainer = new Container("Properties");
            this.addContainer = new Container("Add");
            this.logContainer = new Container("Log ouput");
            //sceneContainer.GetContentPanel().Height = 500;
            Grid.SetColumn(this.sceneContainer, 0);
            Grid.SetRowSpan(this.sceneContainer, 2);
            Grid.SetColumn(this.propertiesContainer, 1);
            Grid.SetColumn(this.addContainer, 1);
            Grid.SetRow(this.addContainer, 1);
            Grid.SetColumn(this.logContainer, 0);
            Grid.SetColumnSpan(this.logContainer, 2);
            Grid.SetRow(this.logContainer, 2);

            this.grid.Children.Add(sceneContainer);
            this.grid.Children.Add(propertiesContainer);
            this.grid.Children.Add(addContainer);
            this.grid.Children.Add(logContainer);
        }

        private void SetupTreeView()
        {
            this.skyBox = new TreeViewItem();
            this.skyBox.Header = "Skybox";
            this.skyBox.Foreground = Brushes.White;

            this.nodes = new TreeViewItem();
            this.nodes.Header = "Nodes";
            this.nodes.Foreground = Brushes.White;

            this.routes = new TreeViewItem();
            this.routes.Header = "Routes";
            this.routes.Foreground = Brushes.White;

            this.scene = new TreeViewItem();
            this.scene.Header = "Scene";
            this.scene.Items.Add(this.skyBox);
            this.scene.Items.Add(this.nodes);
            this.scene.Items.Add(this.routes);
            this.scene.Foreground = Brushes.White;

            this.treeView = new TreeView();
            this.treeView.Items.Add(this.scene);
            this.treeView.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));
            this.treeView.Foreground = Brushes.White;
            this.treeView.BorderBrush = Brushes.Transparent;
            this.treeView.Margin = new Thickness(0, 5, 0, 5);

            this.sceneContainer.GetContentPanel().Children.Add(this.treeView);
        }

        private void SetupTabControl()
        {
            TabControl tabControl = new TabControl();
            tabControl.Items.Add(GetAddNodeTab());
            tabControl.Items.Add(GetAddRouteTab());
            tabControl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E1E"));

            this.addContainer.GetContentPanel().Children.Add(tabControl);
        }

        private TabItem GetAddNodeTab()
        {
            WrapPanel transform = new WrapPanel();
            transform.Children.Add(GetInputField("PosX:", "0", true));
            transform.Children.Add(GetInputField("PosY:", "0", true));
            transform.Children.Add(GetInputField("PosZ:", "0", true));
            transform.Children.Add(GetInputField("Scale:", "1,0", true));
            transform.Children.Add(GetInputField("DirX:", "0", true));
            transform.Children.Add(GetInputField("DirY:", "0", true));
            transform.Children.Add(GetInputField("DirZ:", "0", true));

            CheckBox smoothNormals = new CheckBox();
            smoothNormals.Foreground = Brushes.White;
            smoothNormals.Content = "Smooth normals";
            smoothNormals.Margin = new Thickness(0, 5, 0, 5);

            WrapPanel terrainProperties = new WrapPanel();
            terrainProperties.Children.Add(GetInputField("Width:", "1", true));
            terrainProperties.Children.Add(GetInputField("Depth:", "1", true));
            terrainProperties.Children.Add(GetInputField("MaxHeight:", "1", true));
            terrainProperties.Children.Add(smoothNormals);

            StackPanel terrain = new StackPanel();
            terrain.Children.Add(terrainProperties);
            terrain.Children.Add(GetInputField("Heightmap:", "", false));
            terrain.Visibility = Visibility.Collapsed;
            terrain.Margin = new Thickness(0, 0, 0, 5);

            CheckBox hasTerrain = new CheckBox();
            hasTerrain.Foreground = Brushes.White;
            hasTerrain.Content = "Has terrain";
            hasTerrain.Margin = new Thickness(0, 5, 0, 5);
            hasTerrain.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { terrain.Visibility = Visibility.Visible; });
            hasTerrain.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { terrain.Visibility = Visibility.Collapsed; });

            Button addNodeButton = new Button();
            addNodeButton.Content = "Add node";
            addNodeButton.Foreground = Brushes.White;
            addNodeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            addNodeButton.BorderBrush = Brushes.Transparent;

            StackPanel addNodePanel = new StackPanel();
            addNodePanel.Children.Add(GetInputField("Name: ", "", false));
            addNodePanel.Children.Add(transform);
            addNodePanel.Children.Add(GetInputField("Model: ", "", false));
            addNodePanel.Children.Add(hasTerrain);
            addNodePanel.Children.Add(terrain);
            addNodePanel.Children.Add(addNodeButton);

            addNodeButton.Click += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                string name = ((addNodePanel.Children[0] as StackPanel).Children[1] as TextBox).Text;
                if (!String.IsNullOrEmpty(name))
                {
                    Node node = new Node(name, this.session);

                    //---------------------- Transform ----------------------
                    Vector3 position = new Vector3(float.Parse((((transform.Children[0] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse((((transform.Children[1] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse((((transform.Children[2] as StackPanel).Children[1]) as TextBox).Text));
                    float scale = float.Parse((((transform.Children[3] as StackPanel).Children[1]) as TextBox).Text);
                    Vector3 rotation = new Vector3(float.Parse((((transform.Children[4] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse((((transform.Children[5] as StackPanel).Children[1]) as TextBox).Text),
                                                    float.Parse((((transform.Children[6] as StackPanel).Children[1]) as TextBox).Text));
                    Vr.World.Components.Transform nodeTransform = new Vr.World.Components.Transform(position, scale, rotation);

                    node.SetTransform(nodeTransform);
                    //-------------------------------------------------------

                    //---------------------- Model ----------------------
                    string modelFile = ((addNodePanel.Children[2] as StackPanel).Children[1] as TextBox).Text;
                    if (!String.IsNullOrEmpty(modelFile))
                    {
                        Model model = new Model(modelFile, false);
                        node.SetModel(model);
                    }
                    //---------------------------------------------------

                    //---------------------- Terrain ----------------------
                    string heightMapFile = ((terrain.Children[1] as StackPanel).Children[1] as TextBox).Text;
                    if (!String.IsNullOrEmpty(heightMapFile))
                    {
                        int width = int.Parse(((terrainProperties.Children[0] as StackPanel).Children[1] as TextBox).Text);
                        int depth = int.Parse(((terrainProperties.Children[1] as StackPanel).Children[1] as TextBox).Text);
                        int maxHeight = int.Parse(((terrainProperties.Children[2] as StackPanel).Children[1] as TextBox).Text);
                        Terrain nodeTerrain = new Terrain(width, depth, maxHeight, heightMapFile, (smoothNormals.IsChecked == true) ? true : false, this.session);
                        node.SetTerrain(nodeTerrain);
                    }
                    //---------------------------------------------------

                    Task.Run(() => this.session.GetScene().AddNode(node));
                }
            });

            TabItem addNode = new TabItem();
            addNode.Header = "Add Node";
            addNode.Content = addNodePanel;
            addNode.Foreground = Brushes.White;
            addNode.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            addNode.BorderBrush = Brushes.Transparent;
            return addNode;
        }

        private TabItem GetAddRouteTab()
        {
            StackPanel addRoutePanel = new StackPanel();


            TabItem addRoute = new TabItem();
            addRoute.Header = "Add Route";
            addRoute.Content = addRoutePanel;
            addRoute.Foreground = Brushes.White;
            addRoute.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            addRoute.BorderBrush = Brushes.Transparent;
            return addRoute;
        }

        private StackPanel GetInputField(string header, string text, bool isNumber)
        {
            Label label = new Label();
            label.Content = header;
            label.Foreground = Brushes.White;
            TextBox textBox = new TextBox();
            textBox.Text = text;
            textBox.MinWidth = 50;
            textBox.Foreground = Brushes.White;
            textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236"));
            textBox.BorderBrush = Brushes.Transparent;

            textBox.LostFocus += new RoutedEventHandler((object sender, RoutedEventArgs e) =>
            {
                if(isNumber && (sender as TextBox).Text == "")
                {
                    (sender as TextBox).Text = "0";
                }
            });

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(textBox);
            return stackPanel;
        }

        public void OnDataReceived(byte[] data)
        {
            this.session.OnDataReceived(data);
        }

        public void Log(string text)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.logField.Text += text;
                this.logField.ScrollToEnd();
            }));
        }
    }
}
