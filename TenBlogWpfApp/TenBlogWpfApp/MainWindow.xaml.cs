using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using TenBlogWpfApp.ViewModels;

namespace TenBlogWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private int _i;
        private WindowState _windowState = WindowState.Normal;
        private Rect _normalRect;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            _i = 0;
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
    }
}
