using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace RentACar.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue;
            if (value is string stringValue)
            {
                if (bool.TryParse(stringValue, out boolValue))
                {
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                }
                return stringValue.ToLower() == "true" ? Visibility.Collapsed : Visibility.Visible;
            }
            else if (value is bool)
            {
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value != Visibility.Visible;
        }
    }
}
