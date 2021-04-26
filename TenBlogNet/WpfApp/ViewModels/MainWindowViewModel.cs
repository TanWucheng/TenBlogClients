using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using TenBlogNet.WpfApp.Domain;
using TenBlogNet.WpfApp.Models;
using TenBlogNet.WpfApp.UserControls;
using TenBlogNet.WpfApp.Widget;

namespace TenBlogNet.WpfApp.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///     抽屉导航栏所有项目
        /// </summary>
        private readonly ObservableCollection<NavigationItem> _allItems;

        private ICommand _clearSearchTextBoxCommand;
        private ObservableCollection<NavigationItem> _navItems;
        private string _searchNavKeyword;
        private int _selectedNavIndex;
        private NavigationItem _selectedNavItem;
        private ICommand _switchThemeCommand;
        private ObservableCollection<BlogSearchModel> _blogSearchItems;

        public MainWindowViewModel()
        {
            _allItems = GenerateNavItems();
            FilterItems(null);
            _selectedNavItem = _navItems[0];

            MovePrevCommand = new CustomCommandImplementation(
                _ => SelectedNavIndex--,
                _ => SelectedNavIndex > 0);

            MoveNextCommand = new CustomCommandImplementation(
                _ => SelectedNavIndex++,
                _ => SelectedNavIndex < _allItems.Count - 1);
        }

        public CustomCommandImplementation MovePrevCommand { get; }

        public CustomCommandImplementation MoveNextCommand { get; }

        /// <summary>
        ///     关于按钮点击处理Command
        /// </summary>
        public static ICommand RunAboutCommand => new CustomCommandImplementation(async _ =>
        {
            var view = new AboutDialog();
            await DialogHost.Show(view, "RootDialog");
        });

        /// <summary>
        ///     切换亮暗主题按钮点击处理Command
        /// </summary>
        public ICommand SwitchThemeCommand =>
            _switchThemeCommand ??= new DelegateCommand<ToggleButton>(SwitchThemeAsync);

        /// <summary>
        ///     清除搜索框的内容
        /// </summary>
        public ICommand ClearSearchTextBoxCommand =>
            _clearSearchTextBoxCommand ??= new DelegateCommand<AutoCompleteBox>(ClearSearchTextBox);

        /// <summary>
        ///     选中的抽屉导航栏项目
        /// </summary>
        public NavigationItem SelectedNavItem
        {
            get => _selectedNavItem;
            set
            {
                if (value == null || value.Equals(_selectedNavItem)) return;

                _selectedNavItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNavItem)));
            }
        }

        /// <summary>
        ///     经过筛选的抽屉导航栏项目
        /// </summary>
        public ObservableCollection<NavigationItem> NavItems
        {
            get => _navItems;
            set
            {
                _navItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NavItems)));
            }
        }

        /// <summary>
        ///     抽屉导航栏搜索关键词
        /// </summary>
        public string SearchNavKeyword
        {
            get => _searchNavKeyword;
            set
            {
                _searchNavKeyword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NavItems)));
                FilterItems(_searchNavKeyword);
            }
        }

        /// <summary>
        ///     选中的抽屉导航栏项目的索引
        /// </summary>
        public int SelectedNavIndex
        {
            get => _selectedNavIndex;
            set
            {
                _selectedNavIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedNavIndex)));
            }
        }

        public ObservableCollection<BlogSearchModel> BlogSearchItems
        {
            get => _blogSearchItems;
            set
            {
                _blogSearchItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlogSearchItems)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void SwitchThemeAsync(ToggleButton toggleButton)
        {
            var isDarkTheme = toggleButton.IsChecked == true;
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);

            if (SelectedNavItem.Content is not Home home) return;
            var webView = home.BlogArticleWebView;
            if (webView == null) return;
            var args = isDarkTheme ? "true" : "false";
            await webView.CoreWebView2.ExecuteScriptAsync($"toggleTheme({args});");
        }

        private static void ClearSearchTextBox(AutoCompleteBox textBox)
        {
            textBox.Text = string.Empty;
        }

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        private void FilterItems(string keyword)
        {
            var filteredItems =
                string.IsNullOrWhiteSpace(keyword)
                    ? _allItems
                    : _allItems.Where(i => i.Name.ToLower().Contains(keyword.ToLower()));

            NavItems = new ObservableCollection<NavigationItem>(filteredItems);
        }

        private static ObservableCollection<NavigationItem> GenerateNavItems()
        {
            return new()
            {
                new NavigationItem("主页", new Home())
                {
                    HorizontalScrollBarVisibilityRequirement = ScrollBarVisibility.Disabled,
                    VerticalScrollBarVisibilityRequirement = ScrollBarVisibility.Disabled
                },
                new NavigationItem("联系和反馈", new ContactFeedback())
                {
                    HorizontalScrollBarVisibilityRequirement = ScrollBarVisibility.Disabled,
                    VerticalScrollBarVisibilityRequirement = ScrollBarVisibility.Disabled
                }
            };
        }
    }
}