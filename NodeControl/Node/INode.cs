using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NodeControl
{
    public delegate void ParentChanged(DependencyObject oldParent, DependencyObject current);

    public interface INode
    {
        /// <summary>
        /// Used for drag and drop features
        /// </summary>
        UIElement CreateDefaultNode();

        // Each Usercontrol that implements Node can be used
        Node CurrentNode { get; }
        UIElement Element { get; }

        StackPanel Inputs { get; set; }
        StackPanel Outputs { get; set; }

        Type UICopyClass { get; }

        int Pause { get; set; }
        event MouseButtonEventHandler NodePreviewMouseLeftButtonDown;
        event MouseButtonEventHandler NodeMouseDown;
        event MouseEventHandler NodeMouseMove;
        event MouseButtonEventHandler NodeMouseUp;

        event ParentChanged NodeParentChanged;

        Dictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Is called in the node an describes the data, that has to be handled
        /// </summary>
        object[] Process(object[] ins, Dictionary<string, object> properties);

        /// <summary>
        /// This method uses the data processed before and
        /// allows to display or visualize it.
        /// </summary>
        void Output(object[] processed);

        void SetConnection();

        void SetSelected(bool selected);
    }
}
