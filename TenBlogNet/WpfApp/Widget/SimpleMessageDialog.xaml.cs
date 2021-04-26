namespace TenBlogNet.WpfApp.Widget
{
    /// <summary>
    ///     SimpleMessageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleMessageDialog
    {
        public SimpleMessageDialog()
        {
            InitializeComponent();
        }

        public SimpleMessageDialog(string title, string message)
        {
            InitializeComponent();

            TitleTextBlock.Text = title;
            MessageTextBlock.Text = message;
        }
    }
}