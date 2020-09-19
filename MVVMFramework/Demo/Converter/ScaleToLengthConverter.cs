using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonFramework.MVVM;

namespace MVVMFramework
{
    public class ScaleToLengthConverter : BaseValueConverter<ScaleToLengthConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => (double)value * double.Parse(parameter.ToString());
    }
}
