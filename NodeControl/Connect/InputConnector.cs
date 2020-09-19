using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NodeControl
{
    public class InputConnector : Connector
    {
        public Connection Connection { get; set; }

        public InputConnector(Node node, int type) : base(node, type)
        {

        }

        internal override void OnConnectorClick(object sender, RoutedEventArgs e)
        {
            if (node.isTemplate)
                return;

            if (Connection != null)
            {
                Connection.Dispose();
            }

            if (Connection.Current == null)
            {
                new Connection(null, this);
                return;
            }

            if (IsCompantibleWithCurrentConnection())
            {
                /// Prevent from having duplicate pipes
                if (Connection.Current.OutputConnector.Connections.Any(x => x.InputConnector == this))
                    return;
                Connection.Current.InputConnector = this;
            }
        }

        internal override void OnConnectorMouseEnter(object sender, MouseEventArgs e)
        {
            if (IsCompantibleWithCurrentConnection())
            {
                // Hover Color Here
                BorderBrush = Brushes.White;
            }
        }

        internal override void OnConnectorMouseLeave(object sender, MouseEventArgs e)
        {
            BorderBrush = Brushes.Black;
        }
    }
}
