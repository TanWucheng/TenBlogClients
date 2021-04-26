using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UwpApp.Domain;

namespace UwpApp.Pages
{
    /// <summary>
    ///     可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            //if (selectedTheme != null) ThemeHelper.RootTheme = App.GetEnum<ElementTheme>(selectedTheme);
            if (selectedTheme != null) ThemeHelper.RootTheme = EnumParser<ElementTheme>.Parse(selectedTheme, ElementTheme.Default);
        }

        private void SettingsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeHelper.RootTheme.ToString();
            var buttons = ThemePanel.Children.Cast<RadioButton>();
            var radioButtons = buttons as RadioButton[] ?? buttons.ToArray();
            if (!radioButtons.Any()) return;
            var ratioButton = radioButtons.FirstOrDefault(r => r.Tag.ToString().Equals(currentTheme));
            if (ratioButton != null)
            {
                ratioButton.IsChecked = true;
            }
        }
    }
}