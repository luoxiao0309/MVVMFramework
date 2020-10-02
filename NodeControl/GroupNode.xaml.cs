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
    enum DragResizeType
    {
        None,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom,
        Top,
        Bottom,
        Left,
        Right,
    }

    /// <summary>
    /// GroupControl.xaml 的交互逻辑
    /// </summary>
    public partial class GroupNode : UserControl, INode
    {
        public GroupNode()
        {
            InitializeComponent();
            CurrentNode = new Node(this);
            NodeMouseDown += GroupNodeMouseDown;
            NodeMouseMove += GroupNodeMouseMove;
            NodeMouseUp += GroupNodeMouseUp;

            this.MouseDown += (sender, args) => NodeMouseDown?.Invoke(sender, args);
            this.MouseMove += (sender, args) => NodeMouseMove?.Invoke(sender, args);
            this.MouseUp += (sender, args) => NodeMouseUp?.Invoke(sender, args);
            this.MouseEnter += GroupNodeMouseEnter;
        }

        private double BorderSize = 4;
        public bool IsDraggingToResize { get; private set; } = false;
        DragResizeType _IsDraggingToResizeType = DragResizeType.None;

        private Point Position;
        private Rect _CapturedNodeRect;

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

        public List<Node> IncludeNodes = new List<Node>();
        /// <summary>
        /// 鼠标起始位置
        /// </summary>
        private Point MouseStartPos;

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
        
        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupNodeMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            MouseStartPos = e.GetPosition(CurrentNode.graph);

            if (_IsDraggingToResizeType != DragResizeType.None)
            {
                IsDraggingToResize = true;
                Position = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                _CapturedNodeRect = new Rect(Position, new Size(GroupBorder.ActualWidth, GroupBorder.ActualHeight));
                CurrentNode.dragStart = null;
            }
            else
            {
                var CurrentRect =  CurrentNode.node.Element.GetBoundingBox();
                foreach (var item in CurrentNode.graph.getNodes())
                {
                    var itemRect = item.node.Element.GetBoundingBox();
                    if (CurrentNode!=item)
                    {
                        if (CurrentRect.IsInclude(itemRect))
                        {
                            item.Selected = true;
                            IncludeNodes.Add(item);
                            item.BeginPosition = item.Position;
                        }
                    }
                }
            }
        }

        private void GroupNodeMouseEnter(object sender, MouseEventArgs e)
        {
            if (IsDraggingToResize == false)
            {
                UpdateMouseCursor(e);
            }
        }
        
        /// <summary>
        /// 更新鼠标样式
        /// </summary>
        /// <param name="e"></param>
        void UpdateMouseCursor(MouseEventArgs e)
        {
            var pos = e.GetPosition(GroupBorder);
            if (pos.X < BorderSize)
            {
                // left
                // cursor is inside left vertical border.
                if (pos.Y < BorderSize)
                {
                    // top
                    _IsDraggingToResizeType = DragResizeType.LeftTop;
                    //Mouse.SetCursor(Cursors.SizeNWSE);
                    Cursor = Cursors.SizeNWSE;
                }
                else if (pos.Y > ActualHeight - BorderSize)
                {
                    // bottom
                    _IsDraggingToResizeType = DragResizeType.LeftBottom;
                    //Mouse.SetCursor(Cursors.SizeNESW);
                    Cursor = Cursors.SizeNESW;
                }
                else
                {
                    // left
                    _IsDraggingToResizeType = DragResizeType.Left;
                    //Mouse.SetCursor(Cursors.SizeWE);
                    Cursor = Cursors.SizeWE;
                }
            }
            else if (pos.X > (ActualWidth - BorderSize))
            {
                // right
                // cursor is inside right vertical border.
                if (pos.Y < BorderSize)
                {
                    // top
                    _IsDraggingToResizeType = DragResizeType.RightTop;
                    //Mouse.SetCursor(Cursors.SizeNESW);
                    Cursor = Cursors.SizeNESW;
                }
                else if (pos.Y > ActualHeight - BorderSize)
                {
                    // bottom
                    _IsDraggingToResizeType = DragResizeType.RightBottom;
                    //Mouse.SetCursor(Cursors.SizeNWSE);
                    Cursor = Cursors.SizeNWSE;
                }
                else
                {
                    _IsDraggingToResizeType = DragResizeType.Right;
                    //Mouse.SetCursor(Cursors.SizeWE);
                    Cursor = Cursors.SizeWE;
                }
            }
            else
            {
                // middle
                if (pos.Y < BorderSize)
                {
                    _IsDraggingToResizeType = DragResizeType.Top;
                    //Mouse.SetCursor(Cursors.SizeNS);
                    Cursor = Cursors.SizeNS;
                }
                else if (pos.Y > (ActualHeight - BorderSize))
                {
                    _IsDraggingToResizeType = DragResizeType.Bottom;
                    //Mouse.SetCursor(Cursors.SizeNS);
                    Cursor = Cursors.SizeNS;
                }
                else
                {
                    Cursor = Cursors.Arrow;
                    _IsDraggingToResizeType = DragResizeType.None;
                }
            }
        }

        public void Resize(Point pos)
        {
            switch (_IsDraggingToResizeType)
            {
                case DragResizeType.LeftTop:
                    {
                        var x = (_CapturedNodeRect.Right - pos.X > MinHeight) ? pos.X : Position.X;
                        var y = (_CapturedNodeRect.Bottom - pos.Y > MinHeight) ? pos.Y : Position.Y;
                        Position = new Point(x, y);
                        GroupBorder.Width = Math.Max(MinWidth, _CapturedNodeRect.Right - Position.X);
                        GroupBorder.Height = Math.Max(MinWidth, _CapturedNodeRect.Bottom - Position.Y);
                    }
                    Mouse.SetCursor(Cursors.SizeNWSE);
                    break;
                case DragResizeType.RightTop:
                    {
                        var h = _CapturedNodeRect.Bottom - pos.Y;
                        if (h > MinHeight)
                        {
                            Position = new Point(Position.X, pos.Y);
                            GroupBorder.Height = Math.Max(MinHeight, h);
                        }
                    }
                    GroupBorder.Width = Math.Max(MinWidth, pos.X - _CapturedNodeRect.X);
                    Mouse.SetCursor(Cursors.SizeNESW);
                    break;
                case DragResizeType.LeftBottom:
                    {
                        var w = _CapturedNodeRect.Right - pos.X;
                        if (w > MinWidth)
                        {
                            Position = new Point(pos.X, Position.Y);
                            GroupBorder.Width = Math.Max(MinWidth, w);
                        }
                    }
                    GroupBorder.Height = Math.Max(MinHeight, pos.Y - _CapturedNodeRect.Y);
                    Mouse.SetCursor(Cursors.SizeNESW);
                    break;
                case DragResizeType.RightBottom:
                    GroupBorder.Width = Math.Max(MinWidth, pos.X - _CapturedNodeRect.X);
                    GroupBorder.Height = Math.Max(MinHeight, pos.Y - _CapturedNodeRect.Y);
                    Mouse.SetCursor(Cursors.SizeNWSE);
                    break;
                case DragResizeType.Top:
                    {
                        var h = _CapturedNodeRect.Bottom - pos.Y;
                        if (h > MinHeight)
                        {
                            Position = new Point(Position.X, pos.Y);
                            GroupBorder.Height = Math.Max(MinHeight, h);
                        }
                    }
                    Mouse.SetCursor(Cursors.SizeNS);
                    break;
                case DragResizeType.Bottom:
                    GroupBorder.Height = Math.Max(MinHeight, pos.Y - _CapturedNodeRect.Y);
                    Mouse.SetCursor(Cursors.SizeNS);
                    break;
                case DragResizeType.Left:
                    {
                        var w = _CapturedNodeRect.Right - pos.X;
                        if (w > MinWidth)
                        {
                            Position = new Point(pos.X, Position.Y);
                            GroupBorder.Width = Math.Max(MinWidth, w);
                        }
                    }
                    Mouse.SetCursor(Cursors.SizeWE);
                    break;
                case DragResizeType.Right:
                    GroupBorder.Width = Math.Max(MinWidth, pos.X - _CapturedNodeRect.X);
                    Mouse.SetCursor(Cursors.SizeWE);
                    break;
            }
        }
        
        private void GroupNodeMouseMove(object sender, MouseEventArgs e)
        {
            if (IsDraggingToResize && e.LeftButton == MouseButtonState.Pressed)
            {
                Resize(e.GetPosition(CurrentNode.graph.canvas));
            }

            //移动组节点框
            if (IsDraggingToResize == false)
            {
                UpdateMouseCursor(e);

                if (IncludeNodes.Count>0)
                {
                    Point current = e.GetPosition(CurrentNode.graph.canvas);
                    Vector Deleta = new Vector(current.X - MouseStartPos.X, current.Y - MouseStartPos.Y);
                   
                    foreach (var item in IncludeNodes)
                    {
                        item.Position = item.BeginPosition + Deleta;
                    }
                }
            }
        }


        private void GroupNodeMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsDraggingToResize = false;

            IncludeNodes.Clear();
        }
    }
}
