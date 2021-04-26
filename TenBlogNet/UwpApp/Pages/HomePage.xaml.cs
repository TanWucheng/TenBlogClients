using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using FontAwesome.UWP;
using UwpApp.RssSubscriber;
using UwpApp.ViewModels;
using RefreshContainer = Microsoft.UI.Xaml.Controls.RefreshContainer;
using RefreshRequestedEventArgs = Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs;

namespace UwpApp.Pages
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage
    {
        private EntryViewModel _selectedEntry;
        private readonly HomeViewModel _viewModel;

        public HomePage()
        {
            InitializeComponent();

            // Ensure that the MainPage is only created once, and cached during navigation.
            NavigationCacheMode = NavigationCacheMode.Enabled;

            _viewModel = new HomeViewModel();
            DataContext = _viewModel;

            LoadRssBlogsAsync();
        }

        private Deferral RefreshCompletionDeferral { get; set; }

        private async void LoadRssBlogsAsync(bool doHttpRequest = false)
        {
            var entries = await RssSubscribeService.GetBlogEntries(doHttpRequest: doHttpRequest);
            HomeProgressBar.Visibility = Visibility.Collapsed;
            foreach (var entry in entries)
            {
                List<CategoryViewModel> list = new();
                foreach (var category in entry.Categories)
                {
                    switch (category.Term)
                    {
                        case "Github":
                            {
                                list.Add(new CategoryViewModel { Category = category, FontAwesomeIcon = FontAwesomeIcon.Github });
                                break;
                            }
                        case "笔记":
                            {
                                list.Add(new CategoryViewModel { Category = category, FontAwesomeIcon = FontAwesomeIcon.Book });
                                break;
                            }
                        case ".NET For Android":
                            {
                                list.Add(new CategoryViewModel { Category = category, FontAwesomeIcon = FontAwesomeIcon.Android });
                                break;
                            }
                        case "Statement":
                            {
                                list.Add(new CategoryViewModel
                                { Category = category, FontAwesomeIcon = FontAwesomeIcon.Comments });
                                break;
                            }
                        case ".NET":
                        case "Blazor":
                            {
                                list.Add(new CategoryViewModel
                                { Category = category, FontAwesomeIcon = FontAwesomeIcon.Code });
                                break;
                            }
                        case "HEXO":
                            {
                                list.Add(new CategoryViewModel { Category = category, FontAwesomeIcon = FontAwesomeIcon.Html5 });
                                break;
                            }
                        case "杂谈":
                            {
                                list.Add(new CategoryViewModel { Category = category, FontAwesomeIcon = FontAwesomeIcon.Pencil });
                                break;
                            }
                        case "谚语":
                            {
                                list.Add(new CategoryViewModel
                                { Category = category, FontAwesomeIcon = FontAwesomeIcon.FileWordOutline });
                                break;
                            }
                        case "教程":
                            {
                                list.Add(new CategoryViewModel
                                { Category = category, FontAwesomeIcon = FontAwesomeIcon.GraduationCap });
                                break;
                            }
                        case "PWA":
                        case "Gulp":
                            {
                                list.Add(new CategoryViewModel
                                { Category = category, FontAwesomeIcon = FontAwesomeIcon.Firefox });
                                break;
                            }
                        default:
                            {
                                list.Add(new CategoryViewModel { Category = category, FontAwesomeIcon = FontAwesomeIcon.Codepen });
                                break;
                            }
                    }
                }

                var code = entry.Title.Substring(0, 1);
                var entryViewModel = new EntryViewModel
                {
                    Code = code,
                    Entry = entry,
                    Categories = list
                };
                entryViewModel.PropertyChanged += EntryViewModel_PropertyChanged;
                _viewModel.Items.Add(entryViewModel);
            }
        }

        private void EntryViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsSelected") return;
            if (sender is EntryViewModel { IsSelected: true } entryViewModel) BlogListView.SelectedValue = entryViewModel;
        }

        private void BlogListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BlogListView.SelectedValue is not EntryViewModel selectedValue) return;
            selectedValue.IsSelected = true;
            foreach (var item in _viewModel.Items)
                if (item.Entry.Id != selectedValue.Entry.Id)
                    item.IsSelected = false;
        }

        private void BlogListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the collection item corresponding to the clicked item.
            if (BlogListView.ContainerFromItem(e.ClickedItem) is ListViewItem container)
            {
                // Stash the clicked item for use later. We'll need it when we connect back from the detail page.
                _selectedEntry = container.Content as EntryViewModel;
                if (_selectedEntry != null)
                    _selectedEntry.IsSelected = true;

                // Prepare the connected animation.
                // Notice that the stored item is passed in, as well as the name of the connected element. 
                // The animation will actually start on the Detailed info page.
                BlogListView.PrepareConnectedAnimation("ForwardConnectedAnimation", _selectedEntry, "TitleTextBlock");
            }

            // Navigate to the DetailedInfoPage.
            // Note that we suppress the default animation. 
            Frame.Navigate(typeof(BlogArticlePage), _selectedEntry, new SuppressNavigationTransitionInfo());
        }

        private async void BlogListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == null) return;
            // If the connected item appears outside the viewport, scroll it into view.
            BlogListView.ScrollIntoView(_selectedEntry, ScrollIntoViewAlignment.Default);
            BlogListView.UpdateLayout();

            // Play the second connected animation. 
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("BackConnectedAnimation");
            if (animation == null) return;
            // Setup the "back" configuration if the API is present. 
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
                animation.Configuration = new DirectConnectedAnimationConfiguration();

            await BlogListView.TryStartConnectedAnimationAsync(animation, _selectedEntry, "TitleTextBlock");
        }

        private async void RefreshContainer_OnRefreshRequested(RefreshContainer sender, RefreshRequestedEventArgs args)
        {
            RefreshCompletionDeferral = args.GetDeferral();
            await Task.Delay(1000);

            LoadRssBlogsAsync(true);
            //_viewModel.Items.Add(new EntryViewModel
            //{
            //    Code = "N",
            //    Categories = new List<CategoryViewModel>
            //    {
            //        new()
            //        {
            //            Category = new Category{Scheme = "",Term = "杂谈"},
            //            FontAwesomeIcon = FontAwesomeIcon.Pencil
            //        }
            //    },
            //    Entry = new Entry
            //    {
            //        Title = $"新增文章: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
            //        Published = DateTime.Now,
            //        Updated = DateTime.Now,
            //        Summary = new Summary
            //        {
            //            Content = "<p>秦时明月汉时关，万里长征人未还。</p><p>但使龙城飞将在，不教胡马度阴山。</p>"
            //        }
            //    }
            //});

            RefreshCompletionDeferral.Complete();
            RefreshCompletionDeferral.Dispose();
            RefreshCompletionDeferral = null;
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (RefreshCompletionDeferral != null) return;
            RefreshContainer.RequestRefresh();
        }
    }
}