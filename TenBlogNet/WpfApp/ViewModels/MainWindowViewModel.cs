using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using TenBlogNet.WpfApp.Domain;
using TenBlogNet.WpfApp.UserControls;
using TenBlogNet.WpfApp.Widget;

namespace TenBlogNet.WpfApp.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// 抽屉导航栏所有项目
        /// </summary>
        private readonly ObservableCollection<NavigationItem> _allItems;

        private NavigationItem _selectedItem;
        private ObservableCollection<NavigationItem> _navItems;
        private string _searchKeyword;
        private int _selectedIndex;

        public MainWindowViewModel()
        {
            _allItems = GenerateNavItems();
            FilterItems(null);
            _selectedItem = _navItems[0];

            MovePrevCommand = new CustomCommandImplementation(
                _ => SelectedIndex--,
                _ => SelectedIndex > 0);

            MoveNextCommand = new CustomCommandImplementation(
                _ => SelectedIndex++,
                _ => SelectedIndex < _allItems.Count - 1);
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
                    HorizontalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto
                },
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CustomCommandImplementation MovePrevCommand { get; }

        public CustomCommandImplementation MoveNextCommand { get; }

        /// <summary>
        /// 关于按钮点击处理Command
        /// </summary>
        public static ICommand RunAboutCommand => new CustomCommandImplementation(async _ =>
        {
            var view = new AboutDialog();
            await DialogHost.Show(view, "RootDialog");
        });

        /// <summary>
        /// 选中的抽屉导航栏项目
        /// </summary>
        public NavigationItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null || value.Equals(_selectedItem)) return;

                _selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }

        /// <summary>
        /// 经过筛选的抽屉导航栏项目
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
        /// 抽屉导航栏搜索关键词
        /// </summary>
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NavItems)));
                FilterItems(_searchKeyword);
            }
        }

        /// <summary>
        /// 选中的抽屉导航栏项目的索引
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
            }
        }
    }
}
