using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NodeControl
{
    public class Connection : IDisposable
    {
        public static Connection Current;

        public readonly PathFigure PathFigure;
        public readonly BezierSegment Bezier;
        public readonly Path Path;
        int count = 1;
        public NodeGraph Graph => (InputConnector != null) ? InputConnector.node.graph : OutputConnector.node.graph;

        public object Result = null;

        private InputConnector _inputConnector;
        public InputConnector InputConnector
        {
            get { return _inputConnector; }
            set
            {
                if (_inputConnector == value)
                    return;
                Current = null;
                _inputConnector = value;
                _inputConnector.node.movedEvent += OnInputNodeMoved;
                _inputConnector.Connection = this;

                if (_inputConnector != null)
                {
                    if (_inputConnector.IsLoaded)
                    {
                        SetAnchorPointB(InputConnector.GetAboluteCenterPosition());
                    }
                    else
                    {
                        _inputConnector.Loaded += OnInputConnectorLoaded;
                    }
                }
            }
        }

        private OutputConnector _outputConnector;
        public OutputConnector OutputConnector
        {
            get
            {
                return _outputConnector;
            }
            set
            {
                if (_outputConnector == value)
                    return;

                Current = null;
                _outputConnector = value;
                _outputConnector.node.movedEvent += OnOutputNodeMoved;
                _outputConnector.Connections.Add(this);

                if (_outputConnector != null)
                {
                    if (_outputConnector.IsLoaded)
                    {
                        SetAnchorPointA(OutputConnector.GetAboluteCenterPosition());
                    }
                    else
                        _outputConnector.Loaded += OnOutputConnectorLoaded;
                }
            }
        }

        private void OnOutputConnectorLoaded(object sender, RoutedEventArgs e)
        {
            _outputConnector.Loaded -= OnOutputConnectorLoaded;
            SetAnchorPointA(OutputConnector.GetAboluteCenterPosition());
        }

        private void OnInputConnectorLoaded(object sender, RoutedEventArgs e)
        {
            _inputConnector.Loaded -= OnInputConnectorLoaded;
            SetAnchorPointB(InputConnector.GetAboluteCenterPosition());
        }

        public Connection(OutputConnector output, InputConnector input)
        {
            Bezier = new BezierSegment()
            {
                IsStroked = true
            };

            // Set up the Path to insert the segments
            PathGeometry pathGeometry = new PathGeometry();

            PathFigure = new PathFigure();
            PathFigure.IsClosed = false;
            pathGeometry.Figures.Add(PathFigure);

            this.InputConnector = input;
            this.OutputConnector = output;
            if (input == null || output == null)
                Current = this;

            PathFigure.Segments.Add(Bezier);
            Path = new Path();
            Path.IsHitTestVisible = false;
            Path.Stroke = Graph.context.getPipeColor((input != null) ? input.type : output.type);
            Path.StrokeThickness = 2;
            Path.Data = pathGeometry;

            Graph.canvas.Children.Add(Path);
            ((UIElement)Graph.canvas).MouseMove += Canvas_MouseMove;
        }

        private void SetAnchorPointB(Point point)
        {
            Bezier.Point3 = point;
            switch (Graph.context.orientation)
            {
                case NodeGraphOrientation.LeftToRight:
                    Bezier.Point2 = new Point(Bezier.Point3.X - Graph.pipeStiffness, Bezier.Point3.Y);
                    break;
                case NodeGraphOrientation.RightToLeft:
                    Bezier.Point2 = new Point(Bezier.Point3.X + Graph.pipeStiffness, Bezier.Point3.Y);
                    break;
                case NodeGraphOrientation.UpToBottom:
                    Bezier.Point2 = new Point(Bezier.Point3.X, Bezier.Point3.Y - Graph.pipeStiffness);
                    break;
                case NodeGraphOrientation.BottomToUp:
                    Bezier.Point2 = new Point(Bezier.Point3.X, Bezier.Point3.Y + Graph.pipeStiffness);
                    break;
            }
        }

        private void SetAnchorPointA(Point point)
        {
            PathFigure.StartPoint = point;
            switch (Graph.context.orientation)
            {
                case NodeGraphOrientation.LeftToRight:
                    Bezier.Point1 = new Point(PathFigure.StartPoint.X + Graph.pipeStiffness, PathFigure.StartPoint.Y);
                    break;
                case NodeGraphOrientation.RightToLeft:
                    Bezier.Point1 = new Point(PathFigure.StartPoint.X - Graph.pipeStiffness, PathFigure.StartPoint.Y);
                    break;
                case NodeGraphOrientation.UpToBottom:
                    Bezier.Point1 = new Point(PathFigure.StartPoint.X, PathFigure.StartPoint.Y + Graph.pipeStiffness);
                    break;
                case NodeGraphOrientation.BottomToUp:
                    Bezier.Point1 = new Point(PathFigure.StartPoint.X, PathFigure.StartPoint.Y - Graph.pipeStiffness);
                    break;
            }
        }

        private void OnOutputNodeMoved(Node node)
        {
            SetAnchorPointA(OutputConnector.GetAboluteCenterPosition());
        }

        private void OnInputNodeMoved(Node node)
        {
            SetAnchorPointB(InputConnector.GetAboluteCenterPosition());
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (Current == null)
            {
                ((UIElement)Graph.canvas).MouseMove -= Canvas_MouseMove;
            }

            if (OutputConnector == null)
            {
                SetAnchorPointA(Mouse.GetPosition(Graph.canvas));
            }
            else if (InputConnector == null)
            {
                SetAnchorPointB(Mouse.GetPosition(Graph.canvas));
            }
        }

        public void Dispose()
        {
            if (InputConnector != null)
                InputConnector.Connection = null;
            if (OutputConnector != null)
                OutputConnector.Connections.Remove(this);
            Graph.canvas.Children.Remove(Path);
            ((UIElement)Graph.canvas.Parent).MouseMove -= Canvas_MouseMove;
        }

    }
}
