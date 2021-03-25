using Android.Content;
using Android.Views;
using Google.Android.Material.Snackbar;

namespace TenBlogDroidApp.Utils
{
    public class SnackbarUtil
    {
        public static void Show(Context context, View view, string message, int duration = BaseTransientBottomBar.LengthShort)
        {
            Snackbar.Make(context, view, message, duration).Show();
        }
    }
}
