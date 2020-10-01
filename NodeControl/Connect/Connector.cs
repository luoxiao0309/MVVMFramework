using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NodeControl
{
    public abstract class Connector : Border
    {
        public readonly Node node;
        public readonly int type;

        public Connector(Node node, int type)
        {
            Width = 12;
            Height = 12;
            Background = Brushes.White;
            BorderBrush = Brushes.Black;
            Margin = new Thickness(0, 5, 0, 5);
            BorderThickness = new Thickness(2);
            CornerRadius = new CornerRadius(8);

            this.node = node;
            this.type = type;

            MouseLeftButtonDown += OnConnectorClick;
            MouseEnter += OnConnectorMouseEnter;
            MouseLeave += OnConnectorMouseLeave;
        }

        public Point GetAboluteCenterPosition()
        {
            //var centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);
            //var result = this.TransformToAncestor(node.graph.canvas).Transform(centerPoint);
            //return result;
            //Transform 变换指定点
            return this.TransformToVisual(node.node.Element).Transform(new Point(Canvas.GetLeft(node.node.Element) + 6, Canvas.GetTop(node.node.Element) + 6));
        }

        internal abstract void OnConnectorClick(object sender, RoutedEventArgs e);

        internal abstract void OnConnectorMouseEnter(object sender, MouseEventArgs e);

        internal abstract void OnConnectorMouseLeave(object sender, MouseEventArgs e);

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            Background = node.context.getPipeColor(type);
        }

        public bool IsCompantibleWithCurrentConnection()
        {
            if (Connection.Current == null)
                return false;
            // If input is set
            if (Connection.Current.InputConnector != null)
            {
                if (this is InputConnector || Connection.Current.InputConnector.node == node || Connection.Current.InputConnector.type != type)
                    return false;
            }
            // Otherwise, if output is set
            else
            {
                if (this is OutputConnector || Connection.Current.OutputConnector.node == node || Connection.Current.OutputConnector.type != type)
                    return false;
            }
            return true;
        }

    }
}
