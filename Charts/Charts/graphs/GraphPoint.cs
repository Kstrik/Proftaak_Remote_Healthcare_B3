using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Charts
{
    public class GraphPoint
    {
        public System.Windows.Point Position {
            get { return this.position; }
            set { SetPosition(value); }
        }

        private System.Windows.Point position;
        private object value;

        private Ellipse ellipse;
        private double radius;
        private bool isSelected;

        private Canvas canvas;

        private Label label;

        public GraphPoint(System.Windows.Point position, object value, double radius, ref Canvas canvas)
        {
            this.canvas = canvas;

            this.position = position;
            this.value = value;
            this.radius = radius;

            this.ellipse = new Ellipse();
            this.ellipse.Width = radius * 2;
            this.ellipse.Height = radius * 2;
            this.ellipse.Margin = new System.Windows.Thickness(position.X - radius, position.Y - radius, 0, 0);
            this.ellipse.Fill = Brushes.White;
            canvas.Children.Add(this.ellipse);

            this.label = new Label();
            this.label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            this.label.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            this.label.Margin = new System.Windows.Thickness(position.X + 50, position.Y + 50, 0, 0);
            this.label.Content = value;
            this.label.FontSize = 20;
            this.label.Foreground = Brushes.White;
        }

        public GraphPoint(System.Windows.Point position, object value, ref Canvas canvas)
            : this(position, value, 2, ref canvas) { }

        public void Display()
        {
            this.canvas.Children.Add(this.ellipse);
        }

        public void Remove()
        {
            this.canvas.Children.Remove(this.ellipse);
        }

        public void OnMouseMoved(System.Windows.Point cursorPosition)
        {
            if (Math.Sqrt(Math.Pow(this.position.X - cursorPosition.X, 2) + Math.Pow(this.position.Y - cursorPosition.Y, 2)) <= this.radius)
            {
                this.isSelected = true;
                OnMouseEnter();
            }
            else if (this.isSelected == true)
            {
                this.isSelected = false;
                OnMouseLeave();
            }
        }

        private void OnMouseEnter()
        {
            this.ellipse.Width = (this.radius * 2) + (this.radius / 4);
            this.ellipse.Height = (this.radius * 2) + (this.radius / 4);
            this.ellipse.Margin = new System.Windows.Thickness((position.X - this.radius) - (this.radius / 8), (position.Y - this.radius) - (this.radius / 8), 0, 0);

            if(!this.canvas.Children.Contains(this.label))
                this.canvas.Children.Add(this.label);
        }

        private void OnMouseLeave()
        {
            this.ellipse.Width = (this.radius * 2);
            this.ellipse.Height = (this.radius * 2);
            this.ellipse.Margin = new System.Windows.Thickness(position.X - this.radius, position.Y - this.radius, 0, 0);

            this.canvas.Children.Remove(this.label);
        }

        private void SetPosition(System.Windows.Point position)
        {
            this.position = position;

            if(this.isSelected)
                this.ellipse.Margin = new System.Windows.Thickness((position.X - this.radius) - (this.radius / 8), (position.Y - this.radius) - (this.radius / 8), 0, 0);
            else
                this.ellipse.Margin = new System.Windows.Thickness(position.X - this.radius, position.Y - this.radius, 0, 0);

            this.label.Margin = new System.Windows.Thickness(position.X, (position.Y - this.radius * 2), 0, 0);
        }
    }
}
