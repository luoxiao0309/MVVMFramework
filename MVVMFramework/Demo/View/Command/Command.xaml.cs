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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CommandWindow : Window
    {
        public CommandWindow()
        {
            InitializeComponent();
        }

        private bool CanNavigateToNextTab
        {
            get
            {
                if (tabControl != null)
                {
                    return tabControl.SelectedIndex < tabControl.Items.Count - 1;
                }
                return false;
            }
        }

        private bool CanNavigateToPreviousTab
        {
            get
            {
                if (tabControl != null)
                {
                    return tabControl.SelectedIndex != 0;
                }
                return false;
            }
        }

        private void OnNextPageCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanNavigateToNextTab)
            {
                tabControl.SelectedIndex += 1; e.Handled = true;
            }
        }
        private void OnPreviousPageCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (CanNavigateToPreviousTab)
            {
                tabControl.SelectedIndex -= 1; e.Handled = true;
            }
        }

        private void CanNextPageCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanNavigateToNextTab;
            e.Handled = true;
        }
        private void CanPreviousPageCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanNavigateToPreviousTab;
            e.Handled = true;
        }

        #region 命令
        
        void AddNameExecute()
        {
            //_names.Add(new NameViewModel { UserName = "黄昏前黎明后", CompanyName = "中软易通科技" });
        }



        bool CanAddNameExecute()
        {
            return true;
        }



        //void UpdateNameExecute(NameViewModel name)
        //{
        //    if (name == null) return;
        //    name.CompanyName = "无";
        //}
        
        //bool CanUpdateNameExecute(NameViewModel name)
        //{
        //    return true;
        //}
        
        //public ICommand AddName { get { return new RelayCommand(AddNameExecute, CanAddNameExecute); } }
        //public ICommand UpdateName { get { return new RelayCommand<NameViewModel>(new Action<NameViewModel>(UpdateNameExecute), new Predicate<NameViewModel>(CanUpdateNameExecute)); } }

        #endregion

    }
}

