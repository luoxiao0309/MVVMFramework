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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonFramework.Extensions;

namespace NodeControl
{
    /// <summary>
    /// GroupControl.xaml 的交互逻辑
    /// </summary>
    public partial class GroupNode : UserControl, INode
    {
        public GroupNode()
        {
            InitializeComponent();
            CurrentNode = new Node(this);

            this.MouseDown += (sender, args) => NodeMouseDown?.Invoke(sender, args);
            this.MouseMove += (sender, args) => NodeMouseMove?.Invoke(sender, args);
            this.MouseUp += (sender, args) => NodeMouseUp?.Invoke(sender, args);
        }

        public Node CurrentNode { get; }

        public UIElement Element { get { return this; } }

        public StackPanel Inputs { get; set; }
        public StackPanel Outputs { get; set; }

        public Type UICopyClass { get { return this.GetType(); } }

        public int Pause { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public event MouseButtonEventHandler NodePreviewMouseLeftButtonDown;
        public event MouseButtonEventHandler NodeMouseDown;
        public event MouseEventHandler NodeMouseMove;
        public event MouseButtonEventHandler NodeMouseUp;
        public event ParentChanged NodeParentChanged;

        public UIElement CreateDefaultNode()
        {
            return null;
        }

        public void Output(object[] processed)
        {
            
        }

        public object[] Process(object[] ins, Dictionary<string, object> properties)
        {
            return null;
        }

        public void SetConnection()
        {
            
        }

        public void SetSelected(bool selected)
        {
            if (selected)
            {
                this.GroupBorder.BorderBrush = Brushes.Aqua;
            }
            else
            {
                this.GroupBorder.BorderBrush = Element.GetBrush("#FF888888");
            }
        }

        /// <summary>
        /// Override OnVisualParentChanged and Invoke Event defined in interface
        /// </summary>
        /// <param name="oldParent"></param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            NodeParentChanged?.Invoke(oldParent, this);
        }
    }
}
