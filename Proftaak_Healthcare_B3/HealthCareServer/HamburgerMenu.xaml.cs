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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HealthCareServer
{
    /// <summary>
    /// Interaction logic for HamburgerMenu.xaml
    /// </summary>
    public partial class HamburgerMenu : UserControl
    {
        bool isActive;
        ThicknessAnimation thicknessAnimation;
        Storyboard storyBoard;
        public HamburgerMenu()
        {
            InitializeComponent();
            isActive = false;
            thicknessAnimation = new ThicknessAnimation();
            thicknessAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(400));
            Storyboard.SetTargetProperty(thicknessAnimation, new PropertyPath(Grid.MarginProperty));
            storyBoard = new Storyboard();
            storyBoard.Children.Add(thicknessAnimation);
        }

        private void Menu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isActive = !isActive;

            if (isActive)
            {
                thicknessAnimation.From = new Thickness(0, 0, 0, 0);
                thicknessAnimation.To = new Thickness(menuContent.Width, 0, 0, 0);
            }
            else
            {
                thicknessAnimation.From = new Thickness(menuContent.Width, 0, 0, 0);
                thicknessAnimation.To = new Thickness(0, 0, 0, 0);
            }

            storyBoard.Begin(grid);
        }
    }
}
