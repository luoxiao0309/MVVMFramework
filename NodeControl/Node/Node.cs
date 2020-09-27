using CommonFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NodeControl
{
    public class Node : IDisposable
    {
        public delegate void NodeEventHandler(Node node);
        public event NodeEventHandler movedEvent;

        /// <summary>
        /// parent and wrapper for the Node Data
        /// </summary>
        public INode node { get; private set; }

        public NodeGraph graph { get; private set; }
        //public NodeChest chest { get; private set; }
        public Nullable<Point> dragStart = null;

        //public NodeGraphContext context => (graph != null) ? graph.context : chest.context;
        public NodeGraphContext context
        {
            get
            {
                return graph.context;
            }
        }

        public InputConnector[] getInputs() => Application.Current.Dispatcher.Invoke(() => node.Inputs.Children.OfType<InputConnector>().ToArray());
        public OutputConnector[] getOutputs() => Application.Current.Dispatcher.Invoke(() => node.Outputs.Children.OfType<OutputConnector>().ToArray());

        private bool _selected = false;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                node.SetSelected(_selected);
            }
        }

        //public bool isTemplate => chest != null;
        public bool isTemplate = false;
        /// <summary>
        /// 起始位置
        /// </summary>
        public Point BeginPosition;

        public Node(INode visual)
        {
            this.node = visual;
            initialize();
        }

        private void initialize()
        {
            //Background = (Brush)converter.ConvertFromString("#262626");
            node.NodePreviewMouseLeftButtonDown += Node_PreviewMouseLeftButtonDown;
            node.NodeMouseDown += mouseDown;
            node.NodeMouseMove += mouseMove;
            node.NodeMouseUp += mouseUp;
            // Event if Parent Changed
            node.NodeParentChanged += OnVisualParentChanged;
        }

        Point startPoint;

        private void Node_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        protected void OnVisualParentChanged(DependencyObject oldParent, DependencyObject current)
        {
            for (int i = 0; i < 5; i++)
            {
                current = (current as FrameworkElement)?.Parent;
                if (current == null)
                    break;
                graph = current as NodeGraph;
                //chest = current as NodeChest;
                //if (graph != null || chest != null)
                //    break;
                if (graph != null)
                {
                    break;
                }
            }
            updateOrientation();
        }

        public void updateOrientation()
        {
            //switch (context.orientation)
            //{
            //    case NodeGraphOrientation.LeftToRight:
                   
            //        break;
            //    case NodeGraphOrientation.RightToLeft:
                    
            //        break;
            //    case NodeGraphOrientation.UpToBottom:
                    
            //        break;
            //    case NodeGraphOrientation.BottomToUp:
                    
            //        break;
            //}
            movedEvent?.Invoke(this);
        }

        private bool isPointWithin(Point point)
        {
            if (point.X < 0 || point.Y < 0 || point.X > graph.ActualWidth || point.Y > graph.ActualHeight)
                return false;
            return true;
        }

        private bool _isAboutToBeRemoved = false;
        public bool isAboutToBeRemoved
        {
            get { return _isAboutToBeRemoved; }
            private set
            {
                if (_isAboutToBeRemoved == value)
                    return;

                _isAboutToBeRemoved = value;
                if (_isAboutToBeRemoved)
                {
                    //BorderBrush = Brushes.Red;
                    //BorderThickness = new Thickness(2);
                }
                else
                {
                    //BorderBrush = Brushes.White;
                    //BorderThickness = new Thickness(0);
                }
            }
        }

        public Point Position
        {
            get { return new Point(Canvas.GetLeft(node.Element), Canvas.GetTop(node.Element)); }
            set
            {
                Console.WriteLine("1. X: "+Canvas.GetLeft(node.Element)+"|Y: "+ Canvas.GetLeft(node.Element));

                Canvas.SetLeft(node.Element, value.X);
                Canvas.SetTop(node.Element, value.Y);

                Console.WriteLine("2. X: " + Canvas.GetLeft(node.Element) + "|Y: " + Canvas.GetLeft(node.Element));

                movedEvent?.Invoke(this);
            }
        }

        public void SetPosition(Point point)
        {
            Canvas.SetLeft(node.Element, point.X);
            Canvas.SetTop(node.Element, point.Y);
        }

        public Connector addInput(int type)
        {
            InputConnector dock = new InputConnector(this, type);
            node.Inputs.Children.Add(dock);
            return dock;
        }

        public void removeInput(Connector dock)
        {
            node.Inputs.Children.Remove(dock);
        }

        public OutputConnector addOutput(int type)
        {
            OutputConnector dock = new OutputConnector(this, type);
            node.Outputs.Children.Add(dock);
            return dock;
        }

        public void removeOutput(Connector dock)
        {
            node.Outputs.Children.Remove(dock);
        }

        public void clearInputs()
        {
            foreach (InputConnector input in getInputs())
            {
                if (input.Connection != null)
                {
                    input.Connection.Dispose();
                }
            }
            node.Outputs.Children.Clear();
        }

        public void clearOutputs()
        {
            foreach (OutputConnector output in getOutputs())
            {
                foreach (Connection pipe in output.Connections)
                {
                    pipe.Dispose();
                }
            }

            node.Outputs.Children.Clear();
        }

        public IProperty addProperty<IProperty>(IProperty property)
        {
            node.Properties.Add((property as INodeProperty).Label, (property as INodeProperty));
            return property;
        }

        public int getMaximumDepth()
        {
            InputConnector[] inputs = getInputs();
            int maxDepth = 0;
            foreach (InputConnector input in inputs)
            {
                Node previousNode = input.Connection?.OutputConnector?.node;
                if (previousNode == null)
                    continue;
                int previousMaxDepth = previousNode.getMaximumDepth() + 1;
                if (previousMaxDepth > maxDepth)
                    maxDepth = previousMaxDepth;
            }
            return maxDepth;
        }

        /// <summary>
        /// 遍历过程
        /// </summary>
        public void queryProcess()
        {
            // Makes sure all inputs are ready for processing. Otherwise, cancel.
            InputConnector[] ins = getInputs();
            foreach (InputConnector input in ins)
                if (input.Connection.Result == null)
                    return;

            // Sleep for a time set
            Thread.Sleep(node.Pause);

            object[] results = node.Process(ins.Select(x => x.Connection.Result).ToArray(), node.Properties);

            // Call the Output Visualization
            node.Output(results);

            // Tranfers results to next nodes
            OutputConnector[] outs = getOutputs();
            for (int i = 0; i < outs.Length; i++)
            {
                foreach (Connection pipe in outs[i].Connections)
                {
                    pipe.Result = results[i];
                    pipe.InputConnector.node.queryProcess();
                }
            }
        }

        public void Dispose()
        {
            foreach (InputConnector input in getInputs())
            {
                if (input.Connection != null)
                    input.Connection.Dispose();
            }
            foreach (OutputConnector output in getOutputs())
            {
                foreach (Connection pipe in output.Connections.ToArray())
                {
                    pipe.Dispose();
                }
            }
        }


        private void mouseMove(object sender, MouseEventArgs args)
        {
            if (isTemplate)
            {
                // Get the current mouse position
                Point mousePos = args.GetPosition(null);
                Vector diff = startPoint - mousePos;

                if (args.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("node", node); // TODO: Doublecheck here!
                    System.Windows.DragDrop.DoDragDrop(node.Element, dragData, DragDropEffects.Move);
                }
            }
            else
            {
                if (dragStart.HasValue && args.LeftButton == MouseButtonState.Pressed)
                {
                    if (graph.SelectNodes.Count > 0)
                    {
                        Point current = args.GetPosition(graph.canvas);

                        foreach (var item in graph.SelectNodes)
                        {
                            if (item.dragStart.HasValue)
                            {
                                Point Deleta = new Point(current.X - item.dragStart.Value.X, current.Y - item.dragStart.Value.Y);
                                //移动线
                                item.Position = new Point(item.BeginPosition.X + Deleta.X, item.BeginPosition.Y + Deleta.Y);
                            }
                        }

                        //graph.Stop = true;
                    }
                    else
                    {
                        UIElement element = (UIElement)sender;
                        Point p2 = args.GetPosition(graph.canvas);
                        Position = new Point(p2.X - dragStart.Value.X, p2.Y - dragStart.Value.Y);

                        Console.WriteLine("p2："+ p2+ "|dragStart:"+ dragStart+ "|Position:"+ Position);
                        isAboutToBeRemoved = !isPointWithin(p2);
                        graph.Stop = true;
                    }
                }
            }
        }


        private void mouseUp(object sender, MouseEventArgs args)
        {
            var element = (UIElement)sender;

            if (graph.SelectNodes.Count > 0)
            {
                if (graph.SelectNodes.Contains(this) == false)
                {
                    dragStart = null;
                }
                else
                {
                    Console.WriteLine("包含当前节点");
                }
            }
            else
            {
                dragStart = null;
            }

            element.ReleaseMouseCapture();
            Selected = false;
            if (isAboutToBeRemoved)
                graph.removeNode(this);

            graph.Stop = false;

            if (graph.SelectNodes.Count > 0)
            {
                foreach (var item in graph.SelectNodes)
                {
                    item.Selected = false;
                }
                graph.SelectNodes.Clear();
            }
        }

        private void mouseDown(object sender, MouseEventArgs args)
        {
            var element = (UIElement)sender;

            if (graph.SelectNodes.Count == 0)
            {
                BeginPosition = Position;
                dragStart = args.GetPosition(element);
                Selected = true;
            }
            else
            {
                if (graph.SelectNodes.Contains(this))
                {
                    Console.WriteLine("Node 多个节点 MouseDown...");

                    foreach (var item in graph.SelectNodes)
                    {
                        item.dragStart = args.GetPosition(graph.canvas);
                        item.BeginPosition = item.Position;
                    }
                }
                else
                {
                    Console.WriteLine("Node 单个节点 MouseDown...");

                    foreach (var item in graph.SelectNodes)
                    {
                        item.Selected = false;
                    }
                    graph.SelectNodes.Clear();

                    BeginPosition = Position;
                    dragStart = args.GetPosition(element);
                    Selected = true;
                }
            }

            element.CaptureMouse();
            graph.Stop = true;
        }

    }
}
