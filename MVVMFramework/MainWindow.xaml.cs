using MVVMFramework.Common;
using MVVMFramework.View;
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

namespace MVVMFramework
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ////注册跳转
            //WindowPageHelper.ShowPageHome = () =>
            //{
            //    frame.Navigate(new PageHome());
            //};

            ////跳转至登录页
            //frame.Navigate(new PageLogin());
        }
    }
}
