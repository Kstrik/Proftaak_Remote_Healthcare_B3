﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace SimulatieTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IBikeDataReceiver
    {
        SimulationBike bike;

        public MainWindow()
        {
            InitializeComponent();

            //Page16 page = new Page16(0x19, 0x66, 0x63, 0x03, 0xFF, 3, 4);
            //Page25 page = new Page25(0x7A, 0x49, 0x32, 0x06, 0x00, 0x33);

            //bike = new SimulationBike(this);
            //bike.AddPageSimualtion(page, 10, 500);

            //Page16 page = new Page16(0x19, 0x66, 0x63, 0x03, 0xFF, 3, 4);
            //Page16 page = new Page16(0x19, 0x68, 0x63, 0x02, 0xFF, 3, 4);
            //Page16 page = new Page16(0x19, 0x66, 0x63, 0x03, 0xFF, 0x34);
            Page25 page = new Page25(0x7A, 0x49, 0x32, 0x06, 0x00, 0x33);
            BikeMessage message = new BikeMessage(page);
            txb_Messages.Text = BitConverter.ToString(message.GetBytes());
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bike.ToggleListening();
        }

        public void ReceiveBikeData(byte[] data)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                txb_Messages.Text += BitConverter.ToString(data) + " : " + Encoding.UTF8.GetString(data) + Environment.NewLine;
                //lbl_Test.Content = BitConverter.ToString(data) + " : " + Encoding.UTF8.GetString(data);
                txb_Messages.ScrollToEnd();
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.bike.ToggleListening();
        }
    }
}
