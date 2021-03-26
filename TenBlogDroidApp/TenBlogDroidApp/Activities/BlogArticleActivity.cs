using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using TenBlogDroidApp.Listeners;
using TenBlogDroidApp.Widgets;
using Xamarin.Essentials;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "BlogArticle", Theme = "@style/AppTheme")]
    public class BlogArticleActivity : AppCompatActivity, IFabDisplayListener
    {
        // private MonitorScrollView _scrollView;
        private Toolbar _toolbar;
        private FloatingActionButton _fab;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_blog_article);

            InitToolbar();
            InitScrollView();
            InitFab();
        }

        private void InitToolbar()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_blog_article);
            if (_toolbar == null) return;
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        private void InitScrollView()
        {
            //_scrollView = FindViewById<MonitorScrollView>(Resource.Id.scrollView_blog_article);
            //_scrollView?.AddFabDisplayListener(this);
        }

        private void InitFab()
        {
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab_blog_article);
        }

        public void FabShow()
        {
            _fab.Animate()?.TranslationY(0)?.SetInterpolator(new DecelerateInterpolator(3));
        }

        public void FabHide()
        {
            var marinParams = new ViewGroup.MarginLayoutParams(_fab.LayoutParameters);
            _fab.Animate()
                ?.TranslationY(_fab.Height * 2 + marinParams.BottomMargin)
                ?.SetInterpolator(new AccelerateInterpolator(3));
        }
    }
}