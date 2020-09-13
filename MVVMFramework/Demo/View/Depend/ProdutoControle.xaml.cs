using MVVMFramework.ViewModel;
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

namespace MVVMFramework.View
{
    /// <summary>
    /// Interaction logic for ProdutoControle.xaml
    /// </summary>
    public partial class ProdutoControle : UserControl
    {
        public ProdutoControle()
        {
            InitializeComponent();
            this.DataContext = new ProdutosViewModel();
        }
    }
}
