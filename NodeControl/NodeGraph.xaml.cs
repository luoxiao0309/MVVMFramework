using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommonFramework.Extensions;

namespace NodeControl
{
    /// <summary>
    /// Interaktionslogik für NodeGraph.xaml
    /// </summary>
    public partial class NodeGraph : Border
    {
        public readonly NodeGraphContext context;

        private Point origMouseDownPoint;

        private INode LastDropItemNode = null;

        private bool moveCamera = false;
        private bool CtrlPress = false;
        public RectSelection rectSelection;

        public List<Node> SelectNodes = new List<Node>();
        //private INodeRepresentation LastDropItemNodeRepresenation = null;

        public IWindow window;

        /// <summary>
        /// 选中节点
        /// </summary>
        private bool SelectedNode = false;

        /// <summary>
        /// 管道刚度
        /// </summary>
        private double _pipeStiffness = 50;
        public double pipeStiffness
        {
            get
            {
                return _pipeStiffness;
            }
            set
            {
                if (_pipeStiffness == value)
                    return;
            }
        }

        public NodeGraph(NodeGraphContext context,IWindow window)
        {
            InitializeComponent();
            this.window = window;

            context.propertyChanged += Context_propertyChanged;
            this.context = context;

            
            this.MouseDown += NodeGraph_MouseDown;
            this.DragEnter += Canvas_DragEnter;     //没用    
            this.Drop += NodeGraph_Drop;            //没用

            this.MouseMove += NodeGraph_Move;
            this.MouseUp += NodeGraph_MouseUp;
            this.MouseLeave += NodeGraph_MouseLeave;
            this.AllowDrop = true;
            rectSelection = new RectSelection(this);
        }

        private void Context_propertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case "orientation":
                    foreach (Node node in getNodes())
                    {
                        node.updateOrientation();
                    }
                    break;
            }
        }

        private void NodeGraph_Drop(object sender, DragEventArgs e)
        {
            //if (LastDropItemNode != null || LastDropItemNodeRepresenation != null)
            //    return;

            INode node = e.Data.GetData("node") as INode;
            LastDropItemNode = node;
            //INodeRepresentation representation = e.Data.GetData("node-template") as INodeRepresentation;
            //LastDropItemNodeRepresenation = representation;

            // Fix for double firing event
            Thread disposeThread = new Thread(() =>
            {
                Thread.Sleep(1);
                Dispatcher.Invoke(new Action(() => {
                    LastDropItemNode = null;
                    //LastDropItemNodeRepresenation = null;
                }));
            });
            disposeThread.Start();

            if (node != null)
            {
                var copy = Activator.CreateInstance(node.UICopyClass) as INode;
                addNode(copy.CurrentNode);
                copy.CurrentNode.Position = e.GetPosition(canvas);
            }
            //if (representation != null)
            //{
            //    var copy = Activator.CreateInstance(representation.NodeTemplate.UICopyClass) as INode;
            //    addNode(copy.Node);
            //    copy.Node.position = e.GetPosition(canvas);
            //}
            e.Handled = true;
        }
        
        private void Canvas_DragEnter(object sender, DragEventArgs e)
        {
            INode node = e.Data.GetData("node") as INode;
            if (node == null || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }


        public T addNode<T>(T node) where T : Node
        {
            canvas.Children.Add(node.node.Element);
            node.node.SetConnection();
            return node;
        }

        public bool tryAddNode(Node node)
        {
            if (!canvas.Children.Contains(node.node.Element))
            {
                canvas.Children.Add(node.node.Element);
                node.node.SetConnection();
                return true;
            }
            return false;
        }

        public void removeNode(Node node)
        {
            canvas.Children.Remove(node.node.Element);
            node.Dispose();
        }

        public bool tryRemoveNode(Node node)
        {
            if (canvas.Children.Contains(node.node.Element))
            {
                canvas.Children.Remove(node.node.Element);
                node.Dispose();
                return true;
            }
            return false;
        }

        public void autoArrange()
        {

            Node[] nodes = getNodes();
            Dictionary<int, int> columns = new Dictionary<int, int>();
            int maxMaxDepth = -1;

            for (int i = 0; i < nodes.Length; i++)
            {
                int maxDepth = nodes[i].getMaximumDepth();
                if (maxDepth > maxMaxDepth)
                    maxMaxDepth = maxDepth;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                int maxDepth = nodes[i].getMaximumDepth();

                if (columns.ContainsKey(maxDepth))
                {
                    columns[maxDepth]++;
                }
                else
                {
                    columns.Add(maxDepth, 1);
                }

                switch (context.orientation)
                {
                    case NodeGraphOrientation.LeftToRight:
                        nodes[i].Position = new Point(maxDepth * 350, columns[maxDepth] * 100);
                        break;
                    case NodeGraphOrientation.RightToLeft:
                        nodes[i].Position = new Point((maxMaxDepth - maxDepth) * 350, columns[maxDepth] * 100);
                        break;
                    case NodeGraphOrientation.UpToBottom:
                        nodes[i].Position = new Point(columns[maxDepth] * 270, maxDepth * 150);
                        break;
                    case NodeGraphOrientation.BottomToUp:
                        nodes[i].Position = new Point(columns[maxDepth] * 270, (maxMaxDepth - maxDepth) * 150);
                        break;
                }
            }
        }

        public Node[] getNodes()
        {
            var x = canvas.Children.OfType<INode>();
            List<Node> nodes = new List<Node>();
            foreach (INode node in x)
            {
                nodes.Add(node.CurrentNode);
            }
            return nodes.ToArray();
        }

        public Path[] GetConnections()
        {
            var x = canvas.Children.OfType<Path>();
            List<Path> nodes = new List<Path>();
            foreach (Path node in x)
            {
                nodes.Add(node);
            }
            return nodes.ToArray();
        }

        public void process()
        {
            var nodes = getNodes();
            // Clears all the previous run results
            foreach (Node node in nodes)
            {
                foreach (OutputConnector dock in node.getOutputs())
                {
                    foreach (Connection pipe in dock.Connections)
                    {
                        pipe.Result = null;
                    }
                }
            }
            // Runs !
            foreach (Node node in nodes.Where(x => x.getInputs().Length == 0))
            {
                Thread thread = new Thread(new ThreadStart(() => {
                    node.queryProcess();
                }));
                thread.Start();
            }
        }

        #region 事件
        
        /// <summary>
        /// 只接受鼠标右键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraph_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                moveCamera = true;
                origMouseDownPoint = e.GetPosition(this);
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                foreach (var item in getNodes())
                {
                    if (item.Selected)
                    {
                        SelectedNode = true;
                        break;
                    }
                }

                Console.WriteLine("NodeGraph_MouseDown SelectedNode:"+ SelectedNode);
                if (SelectedNode == false)
                {
                    origMouseDownPoint = e.GetPosition(this);
                    rectSelection.OnStartDrag(origMouseDownPoint);
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                if (Connection.Current != null)
                {
                    Connection.Current.Dispose();
                    Connection.Current = null;

                    ContextMenu.Visibility = Visibility.Hidden;
                }
                else
                {
                    ContextMenu.Visibility = Visibility.Visible;
                }
            }
        }
        
        private void NodeGraph_MouseUp(object sender, MouseButtonEventArgs e)
        {
            moveCamera = false;

            if (SelectedNode == false)
            {
                rectSelection.OnDragEnd();
            }
            SelectedNode = false;
        }

        private void NodeGraph_Move(object sender, MouseEventArgs e)
        {
            if (moveCamera)
            {
                Point current = e.GetPosition(this);
                Vector Deleta = current - origMouseDownPoint;
                origMouseDownPoint = current;

                foreach (var item in getNodes())
                {
                    item.node.Element.MoveElement(Deleta);
                    //移动线
                    item.Position = item.node.Element.GetUIElementPosition();
                }
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Console.WriteLine("NodeGraph_Move SelectedNode:" + SelectedNode);
            }
            
            if (e.LeftButton == MouseButtonState.Pressed && SelectedNode == false)
            {
                var current = e.GetPosition(this);
                Vector Deleta = current - origMouseDownPoint;
                rectSelection.OnDragMoving(Deleta, MouseButton.Left, current, origMouseDownPoint);

                Rect actualRangeRect = new Rect(origMouseDownPoint, current);
                foreach (var item in getNodes())
                {
                    if (actualRangeRect.IsInclude(item.node.Element.GetBoundingBox()))
                    {
                        item.Selected = true;
                    }
                    else if (actualRangeRect.IntersectsWith(item.node.Element.GetBoundingBox()))
                    {
                        item.Selected = true;
                    }

                    if (item.Selected)
                    {
                        SelectNodes.Add(item);
                    }
                }
            }
        }

        private void NodeGraph_MouseLeave(object sender, MouseEventArgs e)
        {
            moveCamera = false;
            rectSelection.OnDragEnd();
        }
        #endregion

        private void AddNode(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            //Point position = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
            Point position = Mouse.GetPosition(canvas);
            window.AddNode(menuItem.Header.ToString(), position);
        }
    }
}
