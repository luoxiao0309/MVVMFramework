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
    public class OutputConnector : Connector
    {
        /// <summary>
        /// Connected Pipes. Can be any number. The data gets distributed to all connected InputDocks
        /// </summary>
        public HashSet<Connection> Connections = new HashSet<Connection>();
        
        public OutputConnector(Node node, int type) : base(node, type) { }

        internal override void OnConnectorClick(object sender, RoutedEventArgs e)
        {
            if (node.isTemplate)
            {
                return;
            }

            if (Connection.Current == null)
            {
                Connection.Current = new Connection(this, null);
                return;
            }

            if (IsCompantibleWithCurrentConnection())
            {
                Connection.Current.OutputConnector = this;
            }
        }

        internal override void OnConnectorMouseEnter(object sender, MouseEventArgs e)
        {
            if (IsCompantibleWithCurrentConnection())
            {
                BorderBrush = Brushes.White;
            }
        }

        internal override void OnConnectorMouseLeave(object sender, MouseEventArgs e)
        {
            BorderBrush = Brushes.Black;
        }
    }
}
