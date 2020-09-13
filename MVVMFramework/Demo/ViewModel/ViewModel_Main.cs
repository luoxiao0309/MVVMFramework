using CommonFramwork;
using MVVMFramework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MVVMFramework.ViewModel
{
    class ViewModel_Main : ViewModelBase
    {
        public Model_Data Data1 { get; set; }//数据引用声明
        public RelayCommand ClickTestCommand { get; set; }//控件绑定的引用声明
        public RelayCommand ClickOpenCommand { get; set; }

        public ViewModel_Main()
        {
            Data1 = new Model_Data();
            ClickTestCommand = new RelayCommand(clickTestCommand);
            ClickOpenCommand = new RelayCommand(clickOpenCommand);
        }
        #region 与页面的数据绑定
        public string Content //单项绑定
        {
            get
            {
                return Data1.Data + Data1.DataTwoWay;
            }
        }
        public string Content_Two //双向绑定,定义UpdateSourceTrigger可以更改实时时间
        {
            get
            {
                return Data1.DataTwoWay;
            }
            set
            {
                Data1.DataTwoWay = value;
                this.OnPropertyChanged("Content");//数据处理后立即更新
            }
        }
        #endregion

        #region 页面控件的绑定
        public void clickTestCommand(object obj)
        {
            MessageBox.Show("该框是按钮控制弹出");
        }
        public void clickOpenCommand(object obj)
        {
            ////MessageBox.Show("");
            //View_MVVMExample view = new View_MVVMExample();//内存管理测试未完成
            //view.DataContext = new ViewModel_MVVMExample();
            //view.Show();
        }
        #endregion
    }
}
