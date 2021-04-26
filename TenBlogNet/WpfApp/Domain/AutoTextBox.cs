using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TenBlogNet.WpfApp.Domain
{
    public class AutoTextBox : TextBox
    {
        public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems",
            typeof(IEnumerable<string>), typeof(AutoTextBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, ListItemsChanged));

        private bool _loaded;

        //private ScrollViewer Host => Template.FindName("PART_ContentHost", this) as ScrollViewer;

        //private UIElement TextBoxView => (from object o in LogicalTreeHelper.GetChildren(Host) select o as UIElement).FirstOrDefault();

        public AutoTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoTextBox),
                new FrameworkPropertyMetadata(typeof(AutoTextBox)));
        }

        private Popup Popup => Template.FindName("PART_Popup", this) as Popup;

        private ListBox ListBox => Template.FindName("PART_ItemList", this) as ListBox;

        public static IEnumerable<string> GetListItems(AutoTextBox o)
        {
            return (IEnumerable<string>)o.GetValue(ListItemsProperty);
        }

        public static void SetListItems(AutoTextBox o, IEnumerable<string> value)
        {
            o.SetValue(ListItemsProperty, value);
        }

        private static void ListItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as AutoTextBox;
            var listItems = e.NewValue as IEnumerable<string>;

            textBox?.InitListBox(listItems);
        }

        public void InitListBox(IEnumerable<string> listItems)
        {
            ListBox.ItemsSource = listItems;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (!_loaded) return;
            Popup.IsOpen = true;
            ListBox.Items.Filter = item =>
                item is string s && s.StartsWith(Text, StringComparison.CurrentCultureIgnoreCase) &&
                !string.Equals(s, Text, StringComparison.CurrentCultureIgnoreCase);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _loaded = true;
            KeyDown += AutoCompleteTextBox_KeyDown;
            PreviewKeyDown += AutoCompleteTextBox_PreviewKeyDown;
            ListBox.PreviewMouseDown +=
                ItemList_PreviewMouseDown;
            ListBox.KeyDown += ItemList_KeyDown;
        }

        private void AutoCompleteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            Popup.IsOpen = false;
            UpdateSource();
        }

        private void AutoCompleteTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Down || ListBox.Items.Count <= 0 || e.OriginalSource is ListBoxItem) return;
            ListBox.Focus();
            ListBox.SelectedIndex = 0;
            if (ListBox.ItemContainerGenerator.ContainerFromIndex(ListBox.SelectedIndex) is ListBoxItem listBoxItem)
                listBoxItem.Focus();
            e.Handled = true;
        }

        private void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is not ListBoxItem item) return;
            Text = item.Content as string ?? string.Empty;
            if (e.Key != Key.Enter) return;
            Popup.IsOpen = false;
            UpdateSource();
        }

        private void ItemList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            {
                if (e.OriginalSource is not TextBlock tb) return;
                Text = tb.Text;
                UpdateSource();
                Popup.IsOpen = false;
                e.Handled = true;
            }
        }

        private void UpdateSource()
        {
            if (GetBindingExpression(TextProperty) != null)
                GetBindingExpression(TextProperty)?.UpdateSource();
        }
    }
}