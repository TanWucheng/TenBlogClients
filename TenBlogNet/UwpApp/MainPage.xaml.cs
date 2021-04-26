using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using UwpApp.Models;
using UwpApp.Pages;
using UwpApp.RssSubscriber.Models;
using UwpApp.ViewModels;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewBackRequestedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs;
using NavigationViewDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemInvokedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs;
using NavigationViewItemSeparator = Microsoft.UI.Xaml.Controls.NavigationViewItemSeparator;

namespace UwpApp
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage
    {
        internal static NavigationView RootNavigationView;

        /// <summary>
        ///     List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        /// </summary>
        //private readonly List<(string tag, Type pageType)> _pages = new()
        //{
        //    ("home", typeof(HomePage)),
        //    ("about", typeof(AboutPage)),
        //    ("music", typeof(Music)),
        //};
        private readonly List<NavPageItem> _pageItems = new()
        {
            new NavPageItem("home", typeof(HomePage)),
            new NavPageItem("about", typeof(AboutPage)),
            new NavPageItem("music", typeof(Music))
        };

        public MainPage()
        {
            InitializeComponent();

            RootNavigationView = NavView;
        }

        private double NavViewCompactModeThresholdWidth => NavView.CompactModeThresholdWidth;

        private void NavView_OnLoaded(object sender, RoutedEventArgs e)
        {
            // You can also add items in code.
            NavView.MenuItems.Add(new NavigationViewItemSeparator());

            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += ContentFrame_Navigated;

            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];

            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("home", new EntranceNavigationTransitionInfo());

            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            KeyboardAccelerators.Add(goBack);

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            KeyboardAccelerators.Add(altLeft);
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            OnBackRequested();
            args.Handled = true;
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            NavViewSearchBox.Visibility = ContentFrame.SourcePageType == typeof(HomePage)
                ? Visibility.Visible
                : Visibility.Collapsed;

            if (ContentFrame.SourcePageType == typeof(BlogArticlePage))
            {
                MainPageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                NavView.SelectedItem = null;
            }
            else
            {
                MainPageScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            //MainPageScrollViewer.VerticalScrollBarVisibility = ContentFrame.SourcePageType == typeof(BlogArticlePage)
            //    ? ScrollBarVisibility.Disabled
            //    : ScrollBarVisibility.Auto;

            if (ContentFrame.SourcePageType == typeof(SettingsPage))
            {
                // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
                NavView.SelectedItem = (NavigationViewItem)NavView.SettingsItem;
                NavView.Header = "设置";
            }
            else if (ContentFrame.SourcePageType != null && ContentFrame.SourcePageType != typeof(BlogArticlePage))
            {
                var item = _pageItems.FirstOrDefault(p => p.PageType == e.SourcePageType);

                var items = GetAllNavItems(NavView.MenuItems.OfType<NavigationViewItem>().ToList());
                NavView.SelectedItem = items.First(x => item is not null && x.Tag.Equals(item.Tag));

                NavView.Header = ((NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            }
        }

        private static IEnumerable<NavigationViewItem> GetAllNavItems(IReadOnlyCollection<NavigationViewItem> topItems)
        {
            var items = new List<NavigationViewItem>();
            items.AddRange(topItems);
            foreach (var topItem in topItems) items.AddRange(GetAllNavItems(topItem));

            return items;
        }

        private static IEnumerable<NavigationViewItem> GetAllNavItems(NavigationViewItem topItem)
        {
            var items = topItem.MenuItems.OfType<NavigationViewItem>().ToList();
            for (var i = 0; i < items.Count; i++) items.AddRange(GetAllNavItems(items[i]));

            return items;
        }

        private void NavView_OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            OnBackRequested();
        }

        private void OnBackRequested()
        {
            if (ContentFrame.SourcePageType == typeof(BlogArticlePage)) return;
            if (!ContentFrame.CanGoBack) return;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                NavView.DisplayMode is NavigationViewDisplayMode.Compact or NavigationViewDisplayMode.Minimal)
                return;

            ContentFrame.GoBack();
        }

        private void NavView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavView_Navigate(string navItemTag, NavigationTransitionInfo transitionInfo)
        {
            Type page;
            if (navItemTag == "settings")
            {
                page = typeof(SettingsPage);
            }
            else
            {
                var item = _pageItems.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                page = item?.PageType;
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the back stack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (page is not null && preNavPageType != page) ContentFrame.Navigate(page, null, transitionInfo);
        }

        private void ContentFrame_OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void NavViewSearchBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (ContentFrame.SourcePageType != typeof(HomePage)) return;
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
            if (ContentFrame.Content is not HomePage { DataContext: HomeViewModel viewModel }) return;
            var suggestions = new List<Entry>();
            var querySplit = sender.Text.Split(" ");
            var matchEntries = viewModel.Items.Where(item =>
            {
                var flag = true;
                foreach (var queryToken in querySplit)
                    if (item.Entry.Title.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                        flag = false;
                return flag;
            }).Select(x => x.Entry);
            suggestions.AddRange(matchEntries);
            if (suggestions.Count > 0)
            {
                var titles = suggestions.OrderByDescending(i =>
                        i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase))
                    .ThenBy(i => i.Title);
                NavViewSearchBox.ItemsSource = titles;
            }
            else
            {
                NavViewSearchBox.ItemsSource = new[] { "未找到相关内容" };
            }
        }

        private void NavViewSearchBox_OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion is not Entry entry) return;
            EntryViewModel viewModel = new()
            {
                Entry = entry
            };
            ContentFrame.Navigate(typeof(BlogArticlePage), viewModel);
        }
    }
}