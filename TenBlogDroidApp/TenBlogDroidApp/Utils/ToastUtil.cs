using Android.Content;
using Android.Widget;

namespace TenBlogDroidApp.Utils
{
    public class ToastUtil
    {
        public static void Show(Context context, string message, ToastLength length = ToastLength.Short)
        {
            Toast.MakeText(context, message, length).Show();
        }
    }
}
