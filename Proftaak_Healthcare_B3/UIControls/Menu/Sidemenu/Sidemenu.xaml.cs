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

namespace UIControls.Menu.Sidemenu
{
    /// <summary>
    /// Interaction logic for Sidemenu.xaml
    /// </summary>
    public partial class Sidemenu : UserControl
    {
        private Menu menu;
        private Window parentWindow;

        private int contentWidth;
        private int menuBarWidth;

        private ThicknessAnimation slideAnimation;
        private Storyboard storyBoard;

        private bool isOpen;

        public Sidemenu(int contentWidth, int menuBarWidth, int animationDuration)
        {
            InitializeComponent();

            this.menu = new Menu();
            this.contentWidth = contentWidth;
            this.menuBarWidth = menuBarWidth;

            this.slideAnimation = new ThicknessAnimation();
            this.slideAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(animationDuration));
            Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(Sidemenu.MarginProperty));
            this.storyBoard = new Storyboard();
            this.storyBoard.Children.Add(this.slideAnimation);

            this.isOpen = false;

            this.Loaded += new RoutedEventHandler(UserControl_Loaded);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.parentWindow = Window.GetWindow(this);
            this.parentWindow.SizeChanged += ParentWindow_SizeChanged;
            //((Grid)this.Parent).SizeChanged += Sidemenu_SizeChanged;
            stk_Content.Width = this.contentWidth;
            stk_MenuBar.Width = this.menuBarWidth;
            grd_Grid.Width = this.contentWidth + this.menuBarWidth;
            this.Margin = new Thickness(-this.contentWidth, 0, 0, 0);
        }

        private void ParentWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Height = e.NewSize.Height;
            stk_Content.Height = this.Height;
            stk_MenuBar.Height = this.Height;
            grd_Grid.Width = this.contentWidth + this.menuBarWidth;
        }

        private void Sidemenu_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Height = e.NewSize.Height;
        }

        private void MenuToggle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(!this.isOpen)
            {
                this.slideAnimation.From = new Thickness(-this.contentWidth, 0, 0, 0);
                this.slideAnimation.To = new Thickness(0, 0, 0, 0);
            }
            else
            {
                this.slideAnimation.From = new Thickness(0, 0, 0, 0);
                this.slideAnimation.To = new Thickness(-this.contentWidth, 0, 0, 0);
            }
            storyBoard.Begin(this);
            this.isOpen = !this.isOpen;
        }
    }
}
