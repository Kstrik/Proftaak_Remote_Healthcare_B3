using System;
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
using HealthcareClient.Bike;
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
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window, IServerDataReceiver, IBikeDataReceiver {

        private Client client;
        private Session session;

        public ClientWindow()
        {
            InitializeComponent();
            this.client = new Client("145.48.6.10", 6666, this, null);
            this.client.Connect();
            GetCurrentSessions();
            ConnectToBike(this);

        }

        private void ConnectToBike(IBikeDataReceiver bikeDataReceiver)
        {
            RealBike bike = new RealBike("00476", bikeDataReceiver);

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

        public void ReceiveBikeData(byte[] data, Bike.Bike bike)
        {
            Dictionary<string, int> translatedData = TacxTranslator.Translate(BitConverter.ToString(data).Split('-'));
            int PageID;
            translatedData.TryGetValue("PageID", out PageID); //hier moet ik van overgeven maar het kan niet anders
            if (25 == PageID)
            {
                byte[] message = parseBikeData(translatedData);
                //Prepare data to be sent to server
                byte[] buffer = new byte[message.Length+1];

                //Send data to server

            }


        }

        public byte[] parseBikeData(Dictionary<string, int> translatedData)
        {
            //TODO: data is niet gesorteerd, alleen Page 25 wordt geparsed
            byte[] message = new byte[10];
            message[0] = 0b10000000 + 0b00000001;
            message[1] = 1; //Heartrate ID
            message[2] = 0; //Heartbeat -- TODO implement BLE HeartrateMonitor
            message[3] = 3; //Power ID
            int power;  translatedData.TryGetValue("InstantaneousPower", out power);
            byte[] Power = BitConverter.GetBytes(power);
            message[3] = Power[0]; message[4] = Power[1]; message[5] = Power[2]; message[6] = Power[3]; //Instantaneous Power
            message[7] = 7; //Snelheid ID
            message[8] = 0; //Snelheid
            message[9] = 9; //Trapritme ID
            int ritme; translatedData.TryGetValue("InstantaneousCadence", out ritme);
            byte[] Cadence = BitConverter.GetBytes(ritme);
            message[10] = Cadence[0]; message[11] = Cadence[1]; message[12] = Cadence[2]; message[13] = Cadence[3]; // InstantaneousCadence
            message[14] = 0; //Check bits
            byte heartRateError = 0b0100000
            message[14] = message[14] + heartRateError;
            return message;
        }
    }
}