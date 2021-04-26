using System.Windows.Media;
using TenBlogNet.WpfApp.Models;

namespace TenBlogNet.WpfApp.Domain
{
    internal class MainWindowSnackBarMessage
    {
        public static void Show(string message, SnackBarMessageType messageType)
        {
            MainWindow.SnackBar.Background = messageType switch
            {
                SnackBarMessageType.Success => new SolidColorBrush(Color.FromArgb(255, 37, 155, 36)),
                SnackBarMessageType.Normal => new SolidColorBrush(Color.FromArgb(255, 66, 66, 66)),
                SnackBarMessageType.Warning => new SolidColorBrush(Color.FromArgb(255, 239, 108, 0)),
                SnackBarMessageType.Error => new SolidColorBrush(Color.FromArgb(255, 229, 28, 35)),
                _ => MainWindow.SnackBar.Background
            };

            if (MainWindow.SnackBar.MessageQueue != null) MainWindow.SnackBar.MessageQueue.Enqueue(message);
        }
    }
}
