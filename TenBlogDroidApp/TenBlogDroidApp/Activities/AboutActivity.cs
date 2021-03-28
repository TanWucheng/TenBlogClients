using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.CoordinatorLayout.Widget;
using TenBlogDroidApp.Utils;
using Xamarin.Essentials;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "About", Theme = "@style/AppTheme")]
    public class AboutActivity : AppCompatActivity
    {
        private CoordinatorLayout _rootView;
        private Toolbar _toolbar;
        private LinearLayout _checkUpdateLayout;
        private TextView _tvVersion;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_about);

            InitRootView();
            InitToolbar();
            InitCheckUpdateLinear();
            InitVersionTextView();
        }

        private void InitRootView()
        {
            _rootView = FindViewById<CoordinatorLayout>(Resource.Id.coordinator_about);
            FontManager.MarkAsIconContainer(_rootView, FontManager.GetTypeface(this, FontManager.FontAwesome), TypefaceStyle.Normal);
        }

        private void InitToolbar()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_about);
            if (_toolbar == null) return;
            SetSupportActionBar(_toolbar);
        }

        private void InitCheckUpdateLinear()
        {
            _checkUpdateLayout = FindViewById<LinearLayout>(Resource.Id.ll_check_update);
            if (_checkUpdateLayout == null) return;
            _checkUpdateLayout.Click += delegate
            {
                ToastUtil.Show(this, "正在检查更新...");
            };
        }

        private void InitVersionTextView()
        {
            _tvVersion = FindViewById<TextView>(Resource.Id.tv_release_version);
            if (_tvVersion == null) return;
            _tvVersion.Text = $"当前版本: {ApkVersionCodeUtil.GetVersionName(this)}";
        }
    }
}