using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using TenBlogNet.WpfApp.Domain;

namespace TenBlogNet.WpfApp.Widget
{
    /// <summary>
    ///     AboutDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AboutDialog
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink link)
                Process.Start(new ProcessStartInfo("cmd", $"/c start {link.NavigateUri.AbsoluteUri}")
                    {CreateNoWindow = true});
        }

        private void AboutDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var attrs = Application.ResourceAssembly.CustomAttributes;
            //var attr = attrs.First(x => x.AttributeType == typeof(AssemblyInformationalVersionAttribute));
            //var versionAttr = attr.ToString();

            var version = AssemblyUtil.GetAssemblyAttr(typeof(AssemblyInformationalVersionAttribute));

            VersionHyperlink.NavigateUri =
                new Uri($"https://github.com/TanWucheng/TenBlogClients/releases/tag/{version}");
            VersionHyperlink.Inlines.Clear();
            VersionHyperlink.Inlines.Add(new Run(version));
        }
    }
}