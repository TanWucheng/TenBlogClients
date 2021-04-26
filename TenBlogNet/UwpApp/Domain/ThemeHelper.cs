using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace UwpApp.Domain
{
    /// <summary>
    ///     Class providing functionality around switching and restoring theme settings
    /// </summary>
    public static class ThemeHelper
    {
        private const string SelectedAppThemeKey = "SelectedAppTheme";

        private static Window _currentApplicationWindow;

        // Keep reference so it does not get optimized/garbage collected
        private static UISettings _uiSettings;

        /// <summary>
        ///     Gets the current actual theme of the app based on the requested theme of the
        ///     root element, or if that value is Default, the requested theme of the Application.
        /// </summary>
        public static ElementTheme ActualTheme
        {
            get
            {
                if (Window.Current.Content is not FrameworkElement rootElement)
                    return EnumParser<ElementTheme>.Parse(Application.Current.RequestedTheme.ToString(),
                        ElementTheme.Default);
                return rootElement.RequestedTheme != ElementTheme.Default
                    ? rootElement.RequestedTheme
                    : EnumParser<ElementTheme>.Parse(Application.Current.RequestedTheme.ToString(),
                        ElementTheme.Default);
            }
        }

        /// <summary>
        ///     Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get
            {
                if (Window.Current.Content is FrameworkElement rootElement) return rootElement.RequestedTheme;

                return ElementTheme.Default;
            }
            set
            {
                if (Window.Current.Content is FrameworkElement rootElement) rootElement.RequestedTheme = value;

                ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey] = value.ToString();
                UpdateSystemCaptionButtonColors();
            }
        }

        public static void Initialize()
        {
            // Save reference as this might be null when the user is in another app
            _currentApplicationWindow = Window.Current;
            var savedTheme = ApplicationData.Current.LocalSettings.Values[SelectedAppThemeKey]?.ToString();

            if (savedTheme != null) RootTheme = EnumParser<ElementTheme>.Parse(savedTheme, ElementTheme.Default);

            // Registering to color changes, thus we notice when user changes theme system wide
            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
        }

        private static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            // Make sure we have a reference to our window so we dispatch a UI change
            _currentApplicationWindow?.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                UpdateSystemCaptionButtonColors);
        }

        public static bool IsDarkTheme()
        {
            if (RootTheme == ElementTheme.Default) return Application.Current.RequestedTheme == ApplicationTheme.Dark;
            return RootTheme == ElementTheme.Dark;
        }

        public static void UpdateSystemCaptionButtonColors()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = IsDarkTheme() ? Colors.White : Colors.Black;
        }
    }
}