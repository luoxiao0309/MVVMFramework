using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeControl
{
    /// <summary>
    /// 方块选择
    /// </summary>
    public class RectSelection
    {
        private Rect _rect;
        private bool _canceled;

        private Rectangle _rectangleView;
        private static Style _defaultStyle;

        public NodeGraph nodeGraph;

        public RectSelection(NodeGraph nodeGraph)
        {
            this.nodeGraph = nodeGraph;
        }

        private Style make_default_style()
        {
            return new Style
            {
                TargetType = typeof(Rectangle),
                Setters =
                {
                    new Setter()
                    {
                        Property = Shape.StrokeProperty,
                        Value = Brushes.DeepSkyBlue,
                    },
                    new Setter()
                    {
                        Property = Shape.FillProperty,
                        Value = Brushes.LightSkyBlue,
                    },
                    new Setter()
                    {
                        Property = UIElement.OpacityProperty,
                        Value = 0.25d,
                    }
                }
            };
        }

        public bool CanDragStart(MouseButton mouse)
        {
            return mouse == MouseButton.Left;
        }

        public void OnStartDrag(Point StartPoint)
        {
            _canceled = false;

            _rectangleView = new Rectangle();
            _rectangleView.Name = "SelectRect";

            if (_defaultStyle is null)
                _defaultStyle = make_default_style();
            _rectangleView.Style = _defaultStyle;

            _rect.X = StartPoint.X;
            _rect.Y = StartPoint.Y;
            Canvas.SetLeft(_rectangleView, StartPoint.X);
            Canvas.SetTop(_rectangleView, StartPoint.Y);

            _rectangleView.Width = 1;
            _rectangleView.Height = 1;

            nodeGraph.canvas.Children.Add(_rectangleView);
        }

        public void OnDragMoving(Vector vector, MouseButton mouseButton, Point CurrentPoint, Point StartPoint)
        {
            if (_canceled)
                return;

            if (vector.Length is 0)
                return;

            if (mouseButton != MouseButton.Left)
                cancel_internal(true);

            var currentPoint = CurrentPoint;

            // 負のをUI座標には指定できないのでUI空間での座標系を再計算する
            {
                _rect.X = Math.Min(currentPoint.X, StartPoint.X);
                _rect.Y = Math.Min(currentPoint.Y, StartPoint.Y);
                _rect.Width = Math.Max(currentPoint.X, StartPoint.X) - _rect.X;
                _rect.Height = Math.Max(currentPoint.Y, StartPoint.Y) - _rect.Y;

                if (_rectangleView != null)
                {
                    Canvas.SetLeft(_rectangleView, _rect.X);
                    Canvas.SetTop(_rectangleView, _rect.Y);
                    _rectangleView.Width = _rect.Width;
                    _rectangleView.Height = _rect.Height;
                }
            }
        }

        public void OnDragEnd()
        {
            cancel_internal(true);
        }

        private void cancel_internal(bool isSelect)
        {
            if (_canceled)
                return;

            _canceled = true;

            if (nodeGraph.canvas.Children.Contains(_rectangleView))
                nodeGraph.canvas.Children.Remove(_rectangleView);

            if (isSelect)
                OnSelect();
        }

        public void OnSelect()
        {
            if (_rect.Width is 0 && _rect.Height is 0)
                return;

            //var selectNodes = Args.Nodes.Where(x => x.ToRect().HitTest(_rect)).ToArray();

            //if (selectNodes.Any())
            //{
            //    var changed = SelectHelper.OnlySelect(Args.Nodes.OfType<ISelectable>().Concat(Args.Connections), selectNodes);
            //    SelectionChangedCommand?.Execute(new SelectionChangedEventArgs(changed, SelectionType.Rect));
            //    return;
            //}

            //if (Args.Connections.Any() is false)
            //    return;

            //var selectConnections = Args.Connections.Where(x => x.HitTestRect(_rect)).ToArray();

            //if (selectConnections.Any())
            //{
            //    var changed = SelectHelper.OnlySelect(Args.Nodes.OfType<ISelectable>().Concat(Args.Connections), selectConnections);
            //    SelectionChangedCommand?.Execute(new SelectionChangedEventArgs(changed, SelectionType.Rect));
            //}
        }
    }
}
