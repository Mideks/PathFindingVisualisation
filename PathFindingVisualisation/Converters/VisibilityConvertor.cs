using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PathFindingVisualisation.Converters
{

    public class VisibilityConvertor : IValueConverter
    {
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (targetType != typeof(Visibility)) 
                throw new ArgumentException("Целевой тип должен быть Visibility", nameof(targetType));
            if (value?.GetType() != typeof(bool))
                throw new ArgumentException("Значение должно быть bool", nameof(value));
            

            var result = bool.TryParse(parameter as string, out bool reverse);
 
            if (!result) 
                throw new ArgumentException("Параметр должен быть bool", nameof(parameter));

            var visible = (bool)value;
            visible = reverse ? !visible : visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotSupportedException();
        }
    }

}
