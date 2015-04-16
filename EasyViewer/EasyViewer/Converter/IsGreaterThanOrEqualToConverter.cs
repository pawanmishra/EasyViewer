using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EasyViewer.Converter
{
    public class IsGreaterThanOrEqualToConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int originalValue = System.Convert.ToInt32(value);
            int compareToValue = System.Convert.ToInt32(parameter);
            return originalValue > compareToValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
