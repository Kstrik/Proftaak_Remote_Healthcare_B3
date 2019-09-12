﻿using Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace NetworkingTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Server server;
        private Client client;

        private LogField serverLogField;
        private LogField clientLogField;

        public MainWindow()
        {
            InitializeComponent();

            this.serverLogField = new LogField(txb_ServerLog);
            this.clientLogField = new LogField(txb_ClientLog);

            this.server = new Server("127.0.0.1", 1330, this.serverLogField);
            //this.client = new Client("127.0.0.1", 1330, this.clientLogField);
            this.client = new Client("145.48.6.10", 6666, this.clientLogField);

            Packet packet = new Packet();
            packet.AddItem("id", "session/list");

            Map map = new Map(256, 256, 1000, @"C:\Users\Kenley Strik\Documents\School\Leerjaar 2019-2020\Periode 1\Proftaak Remote Healthcare\Sprint 2\HeightMap.jpeg");
            img_Map.Source = BitmapToImageSource(map.GetBitmap());
        }

        private void ServerStart_Click(object sender, RoutedEventArgs e)
        {
            this.server.Start();
        }

        private void ClientStart_Click(object sender, RoutedEventArgs e)
        {
            this.client.Connect();
        }

        private void ServerStop_Click(object sender, RoutedEventArgs e)
        {
            this.server.Stop();
        }

        private void ClientStop_Click(object sender, RoutedEventArgs e)
        {
            this.client.Disconnect();
        }

        private void TransmitServer_Click(object sender, RoutedEventArgs e)
        {
            this.server.Transmit(Encoding.UTF8.GetBytes(txb_ServerInput.Text), "");
        }

        private void TransmitClient_Click(object sender, RoutedEventArgs e)
        {
            this.client.Transmit(Encoding.UTF8.GetBytes(txb_ClientInput.Text));
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }

    public class LogField : ILogger
    {
        private TextBox textBox;

        public LogField(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public void Log(string text)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                textBox.Text += text;
            }));
        }
    }
}