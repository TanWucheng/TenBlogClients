using System;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace UwpApp.Pages
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage
    {
        public AboutPage()
        {
            InitializeComponent();

            FlipSide.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnFlipSidePointerReleased), true);
        }

        public string Version
        {
            get
            {
                var version = Package.Current.Id.Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        public string NavigateUrl
        {
            get
            {
                var version = Package.Current.Id.Version;
                return
                    $"https://github.com/TanWucheng/TenBlogClients/releases/tag/{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        private static double NavViewWidth => MainPage.RootNavigationView.CompactModeThresholdWidth;

        private void OnFlipSidePointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var position = e.GetCurrentPoint(FlipSide).Position;
            var v2 = position.ToVector2() - FlipSide.RenderSize.ToVector2() / 2;
            FlipSide.Axis = new Vector2(-v2.Y, v2.X);
        }
    }
}