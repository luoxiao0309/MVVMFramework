using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace CommonFramework.MVVM
{
    /// <summary>
    /// 继承MarkupExtension 可以直接向XAML直接提供服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseValueConverter<T>: MarkupExtension,IValueConverter where T:class,new()
    {
        private static T Converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        => Converter ?? (Converter = new T());

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
