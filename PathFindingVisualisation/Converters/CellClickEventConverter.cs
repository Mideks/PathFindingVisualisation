using PathFindingVisualization.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PathFindingVisualization.Converters
{
    public class CellClickEventConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = ((value as MouseEventArgs)?.OriginalSource as FrameworkElement)?.DataContext as CellViewModel;
            return result ?? CellViewModel.Unset;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
