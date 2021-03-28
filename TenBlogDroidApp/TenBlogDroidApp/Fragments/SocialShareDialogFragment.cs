using Android.Content;
using TenBlogDroidApp.Widgets;

namespace TenBlogDroidApp.Fragments
{
    public class SocialShareDialogFragment : SimpleDialogFragment
    {
        private string _blogurl;

        public SocialShareDialogFragment(Context context, string title) : base(context, title, string.Empty)
        {
            DialogCreateHandler += (sender, e) =>
            {
                var layout = new SocialShareCoordinatorLayout(context, _blogurl);
                layout.ShareMenuClick += delegate
                {
                    Dismiss();
                };
                e.SetView(layout);
            };
        }

        public void SetBlogUrl(string blogUrl)
        {
            _blogurl = blogUrl;
        }
    }
}
