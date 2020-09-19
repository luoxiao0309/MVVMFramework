using CommonFramework;
using CommonFramework.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMFramework.ViewModel
{
    public class ViewModelPropertyChangedEventManger
    {
        public PropertyViewModel VM { get; set; }

        public ViewModelPropertyChangedEventManger(PropertyViewModel vM)
        {
            VM = vM;
            VM.AddPropertyChanged(o => o.SampleTextBox, this.SampleTextBoxChanged);
        }

        public void SampleTextBoxChanged()
        {
            this.VM.SampleTextBlock = this.VM.SampleTextBox;
        }

    }
}
