﻿using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HealthcareServer.Vr;
using HealthcareServer.Vr.World;
using Microsoft.Win32;
using Networking.Client;
using Networking.Server;
using Networking.VrServer;
using Newtonsoft.Json.Linq;
using UIControls;
using UIControls.Menu;

namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServerDataReceiver {

        private Client client;
        private Session session;
        private List<string> sessies;
        private SidemenuWindow sidemenuWindow;
        private Grid gridSession;
        public MainWindow()
        {
            InitializeComponent();
            sidemenuWindow = new SidemenuWindow(new Sidemenu(250, 50, 400, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#323236")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007acc")),
                                                        new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"))));
            this.client = new Client("145.48.6.10", 6666, this, null);
            this.client.Connect();
            GetCurrentSessions();
        }

        private async Task Initialize(string sessionHost)
        {
            this.session = new Session(ref client, sessionHost);
            await this.session.Create();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string host = sessionBox.SelectedItem.ToString();
            Task.Run(() => Initialize(host));
            lblConnected.Content = "Verbonden";
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
                if (isNumber && (sender as TextBox).Text == "")
                {
                    (sender as TextBox).Text = "0";
                }
            });

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(textBox);
            return stackPanel;
        }

        private WrapPanel GetPosDirField()
        {
            WrapPanel posDirField = new WrapPanel();
            posDirField.Children.Add(GetInputField("PosX:", "0", true));
            posDirField.Children.Add(GetInputField("PosY:", "0", true));
            posDirField.Children.Add(GetInputField("PosZ:", "0", true));
            posDirField.Children.Add(GetInputField("DirX:", "0", true));
            posDirField.Children.Add(GetInputField("DirY:", "0", true));
            posDirField.Children.Add(GetInputField("DirZ:", "0", true));
            return posDirField;
        }

        private StackPanel GetComboBoxField(string header, List<string> items)
        {
            Label label = new Label();
            label.Content = header;
            label.Foreground = Brushes.White;
            ComboBox comboBox = new ComboBox();
            comboBox.ItemsSource = items;
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(comboBox);

            return stackPanel;
        }

        public void OnDataReceived(byte[] data)
        {
            if (this.session != null)
            {
                this.session.OnDataReceived(data);
            }
            else
            {
                HandleRecieve(JObject.Parse(Encoding.UTF8.GetString(data)));
            }
        }

        private void HandleRecieve(JObject jsonData)
        {
            if (jsonData.GetValue("id").ToString() == "session/list")
            {
                List<string> sessions = new List<string>();
                foreach (JObject session in jsonData.GetValue("data").ToObject<JToken>().Children())
                {
                    JObject clientInfo = session.GetValue("clientinfo").ToObject<JObject>();

                    sessions.Add(clientInfo.GetValue("host").ToString());
                }

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.sessionBox.ItemsSource = sessions;
                }));
            }
        }

        private void GetCurrentSessions()
        {
            this.client.Transmit(Encoding.UTF8.GetBytes(Session.GetSessionsListRequest().ToString()));
        }

        private async Task RemoveGroundPlane()
        {
            Node node = await this.session.GetScene().FindNode("GroundPlane");
            await node.Delete();
        }

        private void BtnScene_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.Scene)|*.Scene";
            if (openFileDialog.ShowDialog() == true)
            {
                string sceneInput = File.ReadAllText(openFileDialog.FileName);
                lblScene.Content = openFileDialog.FileName;
            }
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
           Task.Run(()=> session.Create());
        }
    }
}