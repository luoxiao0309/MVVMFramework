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

        public float Zoomfactor { get; set; } = 1.1f;
        ScaleTransform _Scale = new ScaleTransform(1.0, 1.0);
        private readonly MatrixTransform _transform = new MatrixTransform();
        private Point MouseStartPos;

        private Point origMouseDownPoint;
        private Point _OrigMouseDownPoint
        {
            get => origMouseDownPoint;
            set
            {
                origMouseDownPoint = value;
            }
        }

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
        private bool selectedNode = false;

        public bool SelectedNode
        {
            get
            {
                return selectedNode;
            }
            set
            {
                selectedNode = value;
            }
        }

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
            this.MouseWheel += PanAndZoomCanvas_MouseWheel;
            this.DragEnter += Canvas_DragEnter;     //没用    
            //this.Drop += NodeGraph_Drop;            //没用

            this.MouseMove += NodeGraph_Move;
            this.MouseUp += NodeGraph_MouseUp;
            this.MouseLeave += NodeGraph_MouseLeave;
            this.AllowDrop = true;
            rectSelection = new RectSelection(this);

            var transfromGroup = new TransformGroup();
            transfromGroup.Children.Add(_Scale);
            canvas.RenderTransform = transfromGroup;
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
                if (node.CurrentNode==null)
                {
                    throw new Exception("空指针");
                }
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
            Console.WriteLine("NodeGraph_MouseDown.......");
            
            if (e.ChangedButton == MouseButton.Middle)
            {
                moveCamera = true;
                _OrigMouseDownPoint = e.GetPosition(this);
                MouseStartPos = e.GetPosition(canvas);

                foreach (var item in getNodes())
                {
                    item.BeginPosition = item.Position;
                }
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                if (SelectedNode == false)
                {
                    _OrigMouseDownPoint = Mouse.GetPosition(canvas);
                    rectSelection.OnStartDrag(_OrigMouseDownPoint);
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
            _OrigMouseDownPoint = new Point(0,0);
        }

        private void NodeGraph_Move(object sender, MouseEventArgs e)
        {
            if (moveCamera)
            {
                Point current = e.GetPosition(canvas);
                Vector Deleta = current - MouseStartPos;
                
                foreach (var item in getNodes())
                {
                    //移动线
                    item.Position = item.BeginPosition+ Deleta;
                }
            }

            if (e.LeftButton == MouseButtonState.Pressed && SelectedNode == false)
            {
                var current = e.GetPosition(canvas);
                
                Vector Deleta = current - _OrigMouseDownPoint;
                rectSelection.OnDragMoving(Deleta, MouseButton.Left, current, _OrigMouseDownPoint);
                
                Rect actualRangeRect = new Rect(_OrigMouseDownPoint, current);

                if (actualRangeRect.Width==0 || actualRangeRect.Height==0|| (actualRangeRect.X==0 && actualRangeRect.Y==0))
                {
                    return;
                }

                var Nodes = getNodes();
                foreach (var item in Nodes)
                {
                    if (actualRangeRect.IsInclude(item.node.Element.GetBoundingBox()))
                    {
                        item.Selected = true;
                    }
                    else if (actualRangeRect.IntersectsWith(item.node.Element.GetBoundingBox()))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }

                    if (item.Selected)
                    {
                        if (SelectNodes.Contains(item)==false)
                        {
                            SelectNodes.Add(item);
                        }
                    }
                    else
                    {
                        if (SelectNodes.Contains(item))
                        {
                            SelectNodes.Remove(item);
                        }
                    }
                }
            }
        }

        private void NodeGraph_MouseLeave(object sender, MouseEventArgs e)
        {
            moveCamera = false;
            rectSelection.OnDragEnd();
        }

        private void PanAndZoomCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta<0)
            {
                _Scale.ScaleX += 0.1;
                _Scale.ScaleY += 0.1;
            }
            else
            {
                _Scale.ScaleX -= 0.1;
                _Scale.ScaleY -= 0.1;
                if (_Scale.ScaleX<=0.1)
                {
                    _Scale.ScaleX = 0.1;
                    _Scale.ScaleY = 0.1;
                }
            }
            //InvalidateVisual();
            
            //float scaleFactor = Zoomfactor;
            //if (e.Delta < 0)
            //{
            //    scaleFactor = 1f / scaleFactor;
            //}

            //Point mousePostion = e.GetPosition(this);

            //Matrix scaleMatrix = _transform.Matrix;
            //scaleMatrix.ScaleAt(scaleFactor, scaleFactor, mousePostion.X, mousePostion.Y);
            //_transform.Matrix = scaleMatrix;

            //foreach (UIElement child in this.canvas.Children)
            //{
            //    double x = Canvas.GetLeft(child);
            //    double y = Canvas.GetTop(child);

            //    double sx = x * scaleFactor;
            //    double sy = y * scaleFactor;

            //    Canvas.SetLeft(child, sx);
            //    Canvas.SetTop(child, sy);

            //    child.RenderTransform = _transform;
            //}
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
