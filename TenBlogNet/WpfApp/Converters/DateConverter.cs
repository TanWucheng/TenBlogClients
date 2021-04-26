using System;
using System.Globalization;
using System.Windows.Data;

namespace TenBlogNet.WpfApp.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
