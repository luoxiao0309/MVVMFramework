using CommonFramwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMFramework.ViewModel
{
    public class PropertyViewModel : ViewModelBase
    {
        public ViewModelPropertyChangedEventManger VmPropsChangedManager { get; set; }
        private string _sampleTextBox;
        private string _sampleTextBlock;

        public PropertyViewModel()
        {
            this.VmPropsChangedManager = new ViewModelPropertyChangedEventManger(this);
        }

        public string SampleTextBox
        {
            get => _sampleTextBox;
            set => SetProperty(ref _sampleTextBox, value, nameof(SampleTextBox));
        }
        public string SampleTextBlock
        {
            get => _sampleTextBlock;
            set => SetProperty(ref _sampleTextBlock, value, nameof(SampleTextBlock));
        }

    }
}
