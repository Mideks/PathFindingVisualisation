using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PathFindingVisualisation.Converters
{
    public class LocationConverter : IValueConverter
    {
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            // Multiply the input value by the scale parameter
            if (value is not int inputValue) return 0;
            var result = double.TryParse(parameter?.ToString(), out var scale);
            scale = result ? scale : 1;

            return inputValue * scale;
        }

        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotSupportedException();
        }
    }
}
