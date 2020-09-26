using NodeControl;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NodeSample
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,IWindow
    {
        private NodeGraph graph;
        //private NodeChest nodeChest;
        private NodeGraphContext context;

        public MainWindow()
        {
            InitializeComponent();

            context = new NodeGraphContext();
            graph = new NodeGraph(context,this);
            graph.pipeStiffness = 0;
            container.Children.Add(graph);

            //nodeChest = new NodeChest(context);
            //SimpleNode sm = new SimpleNode();
            //nodeChest.addNode(sm.Node);
            ////chestContainer.Children.Add(nodeChest);
            //nodeChest.ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            ConstantNode node = new ConstantNode();
            node.CurrentNode.Position = new Point(50, 50);

            DisplayNode node2 = new DisplayNode();
            node2.CurrentNode.Position = new Point(500, 30);

            graph.addNode(node.CurrentNode);
            graph.addNode(node2.CurrentNode);

            new Connection(node.CurrentNode.getOutputs()[0], node2.CurrentNode.getInputs()[0]);
        }

        private void Btn_run_Click(object sender, RoutedEventArgs e)
        {
            graph.autoArrange();

            Thread x = new Thread(() =>
            {
                /*
                while (true)
                {
                    Dispatcher.Invoke(new Action(() => { graph.process(); }));
                    Thread.Sleep(5);
                } */
                Dispatcher.Invoke(new Action(() => { graph.process(); }));
            });
            x.Start();
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {

        }

        void IWindow.AddNode(string name,Point position)
        {
            if (name == "ConstantNode")
            {
                ConstantNode node = new ConstantNode();
                node.CurrentNode.Position = position;
                graph.addNode(node.CurrentNode);
            }
            else if (name == "DisplayNode")
            {
                DisplayNode node = new DisplayNode();
                node.CurrentNode.Position = position;
                graph.addNode(node.CurrentNode);
            }
            else
            {
                GroupNode node = new GroupNode();
                node.CurrentNode.Position = position;
                graph.addNode(node.CurrentNode);
            }
        }
    }
}
