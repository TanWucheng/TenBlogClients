using System;
using Windows.UI.Xaml.Data;

namespace UwpApp.Domain
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime) return $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}