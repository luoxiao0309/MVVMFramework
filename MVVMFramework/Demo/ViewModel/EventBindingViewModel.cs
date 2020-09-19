using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonFramework.MVVM;

namespace MVVMFramework.Demo.ViewModel
{
    class EventBindingViewModel:ViewModelBase
    {
        private double _currentHeight;
        public double CurrentHeight
        {
            get { return _currentHeight; }
            set { SetProperty(ref _currentHeight, value); }
        }
        private double _currentWidth;
        public double CurrentWidth
        {
            get { return _currentWidth; }
            set { SetProperty(ref _currentWidth, value); }
        }

        public RelayCommand MouseDownCommand { get; set; }
        public RelayCommand SizeChangedCommand { get; set; }

        public void Init()
        {
            MouseDownCommand = new RelayCommand(OnMouseDown);
            SizeChangedCommand = new RelayCommand(OnSizeChanged);
        }

        void OnMouseDown(object args)
        {
            if (args == null) args = "null";
            MessageBox.Show(args.ToString());
        }

        private void OnSizeChanged(object args)
        {
            var sizeArgs = args as SizeChangedEventArgs;
            if (sizeArgs != null)
            {
                CurrentHeight = sizeArgs.NewSize.Height;
                CurrentWidth = sizeArgs.NewSize.Width;
            }
        }
    }
}
