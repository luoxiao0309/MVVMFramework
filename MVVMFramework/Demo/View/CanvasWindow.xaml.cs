﻿using System;
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
using System.Windows.Shapes;

namespace MVVMFramework.Demo.View
{
    /// <summary>
    /// CanvasWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CanvasWindow : Window
    {
        public CanvasWindow()
        {
            InitializeComponent();

            TestCanvas.MouseDown += Canvas_MouseDown;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Canvas_MouseDown");
        }
    }
}
