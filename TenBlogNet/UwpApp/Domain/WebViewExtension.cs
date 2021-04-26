using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpApp.Domain
{
    public class WebViewExtension
    {
        public static readonly DependencyProperty UriProperty =
            DependencyProperty.RegisterAttached("Uri", typeof(string), typeof(WebViewExtension),
                new PropertyMetadata(null, UriPropertyChangedCallback));

        public static string GetUri(DependencyObject obj)
        {
            return (string)obj.GetValue(UriProperty);
        }

        public static void SetUri(DependencyObject obj, string value)
        {
            obj.SetValue(UriProperty, value);
        }

        private static void UriPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not WebView webView) return;
            //var adaptive = e.NewValue.ToString().Replace("<img", "<img width=100%");
            webView.NavigateToString(e.NewValue.ToString());
        }
    }
}