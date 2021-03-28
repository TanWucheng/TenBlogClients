using Android.Content;
using Android.Views;
using Google.Android.Material.BottomSheet;
using TenBlogDroidApp.Utils;

namespace TenBlogDroidApp.Widgets
{
    public class SocialShareBottomSheetDialog : BottomSheetDialog
    {
        public SocialShareBottomSheetDialog(Context context) : base(context)
        {
            Window?.AddFlags(WindowManagerFlags.TranslucentStatus);
            SetContentView(new SocialShareCoordinatorLayout(context, Constants.BlogUrl));
            SetCancelable(true);
            SetCanceledOnTouchOutside(true);
        }
    }
}
