using Charts;
using System;
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

namespace ChartsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Random random;
        private Timer timer;

        private LiveChart liveChart;

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;

            this.random = new Random();
            this.timer = new Timer(1000);
            this.timer.Elapsed += Timer_Elapsed;

            this.liveChart = new LiveChart("Livechart", "ChartXAxis", "ChartYAxis", 40, 400, 200, 50, LiveChart.BlueGreenTheme, cnv_Main, false, true, true, true, true, true, true);
            this.MouseMove += MainWindow_MouseMove;

            this.timer.Start();
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            this.liveChart.OnMouseMove(e.GetPosition(cnv_Main));
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.timer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.liveChart.Update(this.random.Next(0, 100));
            }));
        }
    }
}
