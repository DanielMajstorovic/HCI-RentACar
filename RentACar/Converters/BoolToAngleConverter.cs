using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace RentACar.Converters
{
    public class BoolToAngleConverter : IValueConverter
    {
        public BoolToAngleConverter()
        {
            
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isExpanded)
            {
                return isExpanded ? 180 : 0;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}