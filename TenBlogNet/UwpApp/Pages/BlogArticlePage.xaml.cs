using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using UwpApp.ViewModels;

namespace UwpApp.Pages
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BlogArticlePage
    {
        public BlogArticlePage()
        {
            InitializeComponent();

            BlogWebView.Settings.IsJavaScriptEnabled = false;
        }

        public EntryViewModel EntryViewModel { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Store the item to be used in binding to UI
            EntryViewModel = e.Parameter as EntryViewModel;
            if (EntryViewModel is not { IsSelected: true }) return;
            var imageAnimation =
                ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardConnectedAnimation");
            // Connected animation + coordinated animation
            imageAnimation?.TryStart(TitleTextBlock, new UIElement[] { CoordinatedPanel });
        }

        /// <summary>
        ///     Create connected animation back to collection page.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.SourcePageType != typeof(HomePage)) return;
            if (EntryViewModel is { IsSelected: true })
                ConnectedAnimationService.GetForCurrentView()
                    .PrepareToAnimate("BackConnectedAnimation", TitleTextBlock);
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            BlogWebView.Source = new Uri(EntryViewModel.Entry.Link);
        }

        private void BacToListButton_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void GoBackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (BlogWebView.CanGoBack)
                BlogWebView.GoBack();
            else
                BlogWebView.NavigateToString(EntryViewModel.Entry.Summary.Content);
        }

        private void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (BlogWebView.CanGoForward) BlogWebView.GoForward();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            BlogWebView.Refresh();
        }

        private async void BrowserButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"{EntryViewModel.Entry.Link}"));
        }
    }
}