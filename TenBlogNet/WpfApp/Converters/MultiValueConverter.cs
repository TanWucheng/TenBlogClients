using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Web.WebView2.Wpf;

namespace TenBlogNet.WpfApp.Converters
{
    /// <summary>
    ///     用于从Xml中ViewBlogArticleCommand事件命令传递多参数转换
    ///     <a href="https://www.jianshu.com/p/873789538140"></a>
    /// </summary>
    internal class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var link = values[0] as string;
            if (string.IsNullOrEmpty(link) || values[1] is not WebView2 webView) return null;
            var dictionary = new Dictionary<string, WebView2> {{link, webView}};
            return dictionary;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}