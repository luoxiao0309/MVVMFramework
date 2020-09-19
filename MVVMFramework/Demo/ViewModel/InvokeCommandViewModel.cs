using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonFramework.MVVM;

namespace MVVMFramework.ViewModel
{
    /// <summary>
    /// MainWindow.xamlに使用するViewModel
    /// </summary>
    public class InvokeCommandViewModel : ViewModelBase
    {
        private string imagePath;
        /// <summary>
        /// 表示する画像のパスを取得または設定します。
        /// </summary>
        public string ImagePath
        {
            get { return imagePath; }
            set { this.SetProperty(ref this.imagePath, value); }
        }

        private ObservableCollection<ItemViewModel> list;
        public ObservableCollection<ItemViewModel> List
        {
            get { return list; }
            set { this.SetProperty(ref this.list, value); }
        }


        public InvokeCommandViewModel()
        {
            this.List = new ObservableCollection<ItemViewModel>()
            {
                new ItemViewModel(){Name="hoge1", Value=10},
                new ItemViewModel(){Name="hoge2", Value=20},
                new ItemViewModel(){Name="hoge3", Value=30},
                new ItemViewModel(){Name="hoge4", Value=40},
                new ItemViewModel(){Name="hoge5", Value=50},
            };
        }

        ~InvokeCommandViewModel()
        {
            System.Diagnostics.Trace.WriteLine("MainWindowViewModel Destructed.");
        }

        #region コマンドの実装
        private RelayCommand showMessageCommand;
        public RelayCommand ShowMessageCommand
        {
            get { return showMessageCommand = showMessageCommand ?? new RelayCommand(ShowMessage); }
        }
        private void ShowMessage(object obj)
        {
            MessageBox.Show("ShowMessage Command Invoked!!");
        }

        private RelayCommandType<DragEventArgs> dropFileCommand;
        public RelayCommandType<DragEventArgs> DropFileCommand
        {
            get { return dropFileCommand = dropFileCommand ?? new RelayCommandType<DragEventArgs>(DropFile); }
        }

        private void DropFile(DragEventArgs parameter)
        {
            var fileInfos = parameter.Data.GetData(DataFormats.FileDrop) as string[];
            if (fileInfos != null)
            {
                // ドロップされたファイルの1番目の要素のパスを、画面表示用のプロパティに設定
                this.ImagePath = fileInfos[0];
            }
        }
        #endregion
    }


    public class ItemViewModel : ViewModelBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { this.SetProperty(ref this.name, value); }
        }

        private int value;
        public int Value
        {
            get { return value; }
            set { this.SetProperty(ref this.value, value); }
        }


        private RelayCommand showValueCommand;
        public RelayCommand ShowValueCommand
        {
            get { return showValueCommand = showValueCommand ?? new RelayCommand(ShowValue); }
        }

        private void ShowValue(object obj)
        {
            var msg = string.Format("{0}: {1}", this.Name, this.Value);
            MessageBox.Show(msg);
        }
    }
}
