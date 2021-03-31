using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using AndroidX.AppCompat.App;
using Com.Tencent.Smtt.Sdk;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using TenBlogDroidApp.Fragments;
using TenBlogDroidApp.Listeners;
using TenBlogDroidApp.Models;
using TenBlogDroidApp.Utils;
using TenBlogDroidApp.Widgets;
using Xamarin.Essentials;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "BlogArticle", Theme = "@style/AppTheme")]
    public class BlogArticleActivity : AppCompatActivity, IFabDisplayListener
    {
        private MonitorScrollView _scrollView;
        private Toolbar _toolbar;
        private FloatingActionButton _fab;
        private AppBarLayout _appbarLayout;
        private WebView _webView;
        private TbsListener _tbsListener;
        private PreInitCallback _preInitCallback;
        private string _blogArticleUrl;
        private SocialShareDialogFragment _dialogFragment;
        private SimpleProgressDialogFragment _progressDialogFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            _blogArticleUrl = Intent?.GetStringExtra(Constants.BlogArticleUrl);

            SetContentView(Resource.Layout.activity_blog_article);

            InitToolbar();
            InitScrollView();
            InitFab();
            InitWebView();
        }

        private void InitToolbar()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_blog_article);
            if (_toolbar == null) return;
            SetSupportActionBar(_toolbar);
            SupportActionBar.Title = "文章详情";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        private void InitScrollView()
        {
            _appbarLayout = FindViewById<AppBarLayout>(Resource.Id.appbar_blog_article);
            _scrollView = FindViewById<MonitorScrollView>(Resource.Id.scrollView_blog_article);
            if (_scrollView == null) return;
            _scrollView.AddFabDisplayListener(this);
            _scrollView.TranslucentChange += ScrollView_TranslucentChange;
        }

        private void ScrollView_TranslucentChange(object sender, float e)
        {
            _appbarLayout.Alpha = e;
        }

        private void InitFab()
        {
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab_blog_article);
            _fab.Click += delegate
            {
                _scrollView.ScrollTo(0, 0);
            };
        }

        private void InitWebView()
        {
            ShowProgressDialogAsync();
            _webView = FindViewById<WebView>(Resource.Id.webView_blog_article);
            if (_webView == null) return;
            _preInitCallback = new PreInitCallback();
            _tbsListener = new TbsListener();

            QbSdk.DownloadWithoutWifi = true;
            QbSdk.SetTbsListener(_tbsListener);
            QbSdk.InitX5Environment(this, _preInitCallback);

            _webView.WebViewClient = new SimpleWebViewClient();
            // 网站使用了3D背景动画，启用js会造成WebView性能开销过大，影响流畅性和稳定性
            //_webView.Settings.JavaScriptEnabled = true;
            _webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            _webView.LoadUrl(string.IsNullOrWhiteSpace(_blogArticleUrl) ? Constants.Blog404Url : _blogArticleUrl);
        }

        private async void ShowProgressDialogAsync()
        {
            _progressDialogFragment = SimpleProgressDialogFragment.NewInstance("博文拼命加载中...");
            _progressDialogFragment.Show(SupportFragmentManager, "ProgressDialogFragment");
            await Task.Delay(1000);
            _progressDialogFragment.Dismiss();
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

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && _webView.CanGoBack())
            {
                _webView.GoBack();
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.blog_article, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.blog_article_share:
                    {
                        if (_dialogFragment == null)
                        {
                            _dialogFragment = new SocialShareDialogFragment(this, "社交分享");
                        }
                        _dialogFragment.SetBlogUrl(_webView.Url);
                        _dialogFragment.Show(SupportFragmentManager, "SocialShareDialogFragment");
                        break;
                    }
            }

            return base.OnOptionsItemSelected(item);
        }

        private class TbsListener : Java.Lang.Object, ITbsListener
        {
            public void OnDownloadFinish(int p0)
            {
            }

            public void OnDownloadProgress(int p0)
            {
            }

            public void OnInstallFinish(int p0)
            {
            }
        }
        private class PreInitCallback : Java.Lang.Object, QbSdk.IPreInitCallback
        {
            public void OnCoreInitFinished()
            {
            }

            public void OnViewInitFinished(bool p0)
            {
            }
        }
    }
}