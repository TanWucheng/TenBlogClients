using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Svg2Xaml;

namespace TenBlogNet.WpfApp.Converters
{
    internal class SvgToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return null;
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var path = directory + parameter;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return SvgReader.Load(stream);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
