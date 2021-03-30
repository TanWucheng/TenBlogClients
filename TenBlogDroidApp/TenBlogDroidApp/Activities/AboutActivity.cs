using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Util;
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
        private TextView _tvCopyRight;
        private TextView _tvWhoami;
        private LinearLayout _whoamiLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_about);

            InitRootView();
            InitToolbar();
            InitCheckUpdateLinear();
            InitVersionTextView();
            InitCopyRightTextView();
            InitWhoamiLayout();
        }

        //private void StartTransitionActivity(Type target, Pair[] pairs, string link = null)
        //{
        //    Intent intent = new(this, target);
        //    if (!string.IsNullOrWhiteSpace(link))
        //    {
        //        intent.PutExtra(Constants.BlogArticleUrl, link);
        //    }
        //    ActivityOptionsCompat transitionActivityOptions = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, pairs);
        //    StartActivity(intent, transitionActivityOptions.ToBundle());
        //}

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
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
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
            _tvVersion.Text = $"v{ApkVersionCodeUtil.GetVersionName(this)}";
        }

        private void InitCopyRightTextView()
        {
            _tvCopyRight = FindViewById<TextView>(Resource.Id.tv_copyright);
            if (_tvCopyRight == null) return;
            _tvCopyRight.Text = Resources.GetString(Resource.String.fa_copyright, DateTime.Now.Year);
        }

        private void InitWhoamiLayout()
        {
            _tvWhoami = FindViewById<TextView>(Resource.Id.tv_whoami);
            _whoamiLayout = FindViewById<LinearLayout>(Resource.Id.ll_whoami);
            if (_whoamiLayout == null) return;
            _whoamiLayout.Click += delegate
            {
                var sharePairs = new Pair[1] { new Pair(_tvWhoami, Resources.GetString(Resource.String.transition_whoami)) };
                var pairs = TransitionUtil.CreateSafeTransitionParticipants(this, true, sharePairs);
                Intent intent = new(this, typeof(StatementActivity));
                ActivityOptionsCompat transitionActivityOptions = ActivityOptionsCompat.MakeSceneTransitionAnimation(this, pairs);
                StartActivity(intent, transitionActivityOptions.ToBundle());
            };
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }
    }
}