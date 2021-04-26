using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TenBlogNet.WpfApp.Converters
{
    public class CornerRadiusToThickness : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new Thickness(0);
            if (value is CornerRadius cornerRadius)
            {
                result = new Thickness(cornerRadius.TopLeft, 0, cornerRadius.TopRight, 0);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}