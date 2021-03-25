using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace TenBlogNet.WpfApp.Widget
{
    /// <summary>
    /// AboutDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AboutDialog : UserControl
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link)
            {
                // Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
                Process.Start(new ProcessStartInfo("cmd", $"/c start {link.NavigateUri.AbsoluteUri}") { CreateNoWindow = true });
            }
        }
    }
}
