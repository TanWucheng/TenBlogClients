using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UwpApp.ViewModels;

namespace UwpApp.Pages
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Music
    {
        private readonly MusicViewModel _viewModel;

        public Music()
        {
            InitializeComponent();
            _viewModel = new MusicViewModel
            {
                Systems = new ObservableCollection<string>(new[] { "Windows", "macOS", "Linux", "Android", "iOS" })
            };

            DataContext = _viewModel;
        }

        private Deferral RefreshCompletionDeferral { get; set; }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshContainer.RequestRefresh();
        }

        private async void RefreshContainer_OnRefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            RefreshCompletionDeferral = args.GetDeferral();
            await Task.Delay(1000);
            _viewModel.Systems.Add($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            RefreshCompletionDeferral.Complete();
            RefreshCompletionDeferral.Dispose();
            RefreshCompletionDeferral = null;
        }
    }
}