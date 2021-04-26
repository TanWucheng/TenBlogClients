using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using TenBlogNet.WpfApp.Models;
using TenBlogNet.WpfApp.UserControls;
using TenBlogNet.WpfApp.ViewModels;

namespace TenBlogNet.WpfApp
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        internal static Snackbar SnackBar;
        internal static DialogHost MainDialog;
        internal static MainWindow RootWindow;

        private int _i;
        private Rect _normalRect;
        private WindowState _windowState = WindowState.Normal;

        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            SnackBar = MainSnackBar;
            MainDialog = RootDialog;

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            _i = 0;

            RootWindow = this;
        }

        private void ToolBar_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _i += 1;
            var timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 300)
            };
            timer.Tick += (_, _) =>
            {
                timer.IsEnabled = false;
                _i = 0;
            };
            timer.IsEnabled = true;
            if (_i % 2 != 0) return;
            timer.IsEnabled = false;
            _i = 0;
            switch (_windowState)
            {
                case WindowState.Normal:
                    {
                        var packIcon = new PackIcon
                        {
                            Kind = PackIconKind.WindowRestore
                        };
                        ButtonWinMax.Content = packIcon;
                        _normalRect = new Rect(Left, Top, Width, Height);
                        Left = 0;
                        Top = 0;
                        var rect = SystemParameters.WorkArea;
                        Width = rect.Width;
                        Height = rect.Height;
                        _windowState = WindowState.Maximized;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        var packIcon = new PackIcon
                        {
                            Kind = PackIconKind.WindowMaximize
                        };
                        ButtonWinMax.Content = packIcon;
                        Left = _normalRect.Left;
                        Top = _normalRect.Top;
                        Width = _normalRect.Width;
                        Height = _normalRect.Height;
                        _windowState = WindowState.Normal;
                        break;
                    }
            }
        }

        private void ToolBar_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void ButtonWinMin_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ButtonWinMax_OnClick(object sender, RoutedEventArgs e)
        {
            switch (_windowState)
            {
                case WindowState.Normal:
                    {
                        var packIcon = new PackIcon
                        {
                            Kind = PackIconKind.WindowRestore
                        };
                        ButtonWinMax.Content = packIcon;
                        _normalRect = new Rect(Left, Top, Width, Height);
                        Left = 0;
                        Top = 0;
                        var rect = SystemParameters.WorkArea;
                        Width = rect.Width;
                        Height = rect.Height;
                        _windowState = WindowState.Maximized;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        var packIcon = new PackIcon
                        {
                            Kind = PackIconKind.WindowMaximize
                        };
                        ButtonWinMax.Content = packIcon;
                        Left = _normalRect.Left;
                        Top = _normalRect.Top;
                        Width = _normalRect.Width;
                        Height = _normalRect.Height;
                        _windowState = WindowState.Normal;
                        break;
                    }
            }
        }

        private void ButtonWinClose_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void SearchTextBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchTextBox.SelectedItem is not BlogSearchModel model) return;
            if (Home.HomeInstance != null)
            {
                Home.HomeInstance.BlogArticleWebView.Source = new Uri(model.Link);
            }
        }

        private void NavItemsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.SelectedNavIndex != 0)
            {
                Transitioner.MoveFirstCommand.Execute(null, MainTransitioner);
                SearchBlogButton.Visibility = Visibility.Hidden;
            }
            else
            {
                SearchBlogButton.Visibility = Visibility.Visible;
            }
        }
    }
}