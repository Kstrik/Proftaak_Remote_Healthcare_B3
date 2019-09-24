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
using HealthcareClient.BikeConnection;
using HealthcareClient.ServerConnection;
using HealthcareServer.Vr;
using HealthcareServer.Vr.World;
using Microsoft.Win32;
using Networking.Client;
using Newtonsoft.Json.Linq;



namespace HealthcareClient
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>

    public partial class ClientWindow : Window, IServerDataReceiver, IBikeDataReceiver, IClientMessageReceiver
    {
        [Flags] public enum CheckBits { Sessie = 0b0001000, BikeError = 0b0000100, HeartBeatError = 0b00000010, VRError = 0b00000001 };
        private Client client;
        private Session session;
        private DataManager dataManager;
        public ClientWindow()
        {
            InitializeComponent();
            this.client = new Client("145.48.6.10", 6666, this, null);
            this.client.Connect();
            dataManager = new DataManager(this);
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
            Task.Run(() => session.Create());
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
                dataManager.addPage25(power, cadence);
            }
            else if (16 == PageID)
            {
                int speed; translatedData.TryGetValue("speed", out speed);
                dataManager.addPage16(speed);
            }
        }

        private void ReceiveHeartbeatData(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called from datamanager. Parses a complete ClientMessage into a packet to be sent via TCP
        /// </summary>
        void IClientMessageReceiver.handleClientMessage(ClientMessage clientMessage)
        {
            byte clientID = 0b00000001; // message is from a client
            byte Checkbits = (byte)CheckBits.HeartBeatError; //heartbeat not implemented yet
            byte[] message = clientMessage.toByteArray();
            byte[] messageLength = BitConverter.GetBytes(message.Length);
            message.Prepend(messageLength[3]);
            message.Prepend(clientID);
            message.Append(Checkbits);
            Send( message);
        }

        private void Send(byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}