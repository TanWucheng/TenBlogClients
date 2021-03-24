using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Transitions;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Infideap.DrawerBehavior;
using Plugin.Permissions;
using Ten.Droid.Library.Extensions;
using Ten.Droid.Library.RecyclerView.Adapters;
using Ten.Droid.Library.Utils;
using TenBlogDroidApp.Adapters;
using TenBlogDroidApp.Fragments;
using TenBlogDroidApp.Listeners;
using TenBlogDroidApp.RssSubscriber.Models;
using TenBlogDroidApp.Services;
using TenBlogDroidApp.Utils;
using TenBlogDroidApp.Widgets;
using Xamarin.Essentials;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener,
        IDialogFragmentCallBack, IFabDisplayListener
    {
        private AnimatorAdapter _animatorAdapter;
        private BlogRecyclerViewAdapter _blogAdapter;
        private RecyclerView _blogRecyclerView;
        private SimpleProgressDialogFragment _dialogFragment;
        private Advance3DDrawerLayout _drawer;
        private List<Entry> _entries;
        private FloatingActionButton _fab;
        private string _keywords = string.Empty;
        private LinearLayoutManager _layoutManager;
        private RemovableEditText _searchEditText;
        private StandardRecyclerViewAdapter<string> _searchResultAdapter;
        private RecyclerView _searchResultRecyclerView;
        private SwipeRefreshLayout _swipeRefreshLayout;
        private Toolbar _toolbar;
        private SocialShareBottomSheetDialog _bottomSheetDialog;
        private CoordinatorLayout _coordinatorLayout;

        public void DialogShow()
        {
            _dialogFragment?.Show(SupportFragmentManager, "ProgressDialogFragment");
        }

        public void DialogDismiss()
        {
            _dialogFragment?.Dismiss();
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

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case Resource.Id.nav_share:
                    {
                        _bottomSheetDialog ??= new SocialShareBottomSheetDialog(this);

                        _bottomSheetDialog.Show();
                        break;
                    }
                case Resource.Id.nav_contact_feedback:
                    {
                        Intent intent = new(this, typeof(ContactFeedbackActivity));
                        var activityOptionsCompat = ActivityOptionsCompat.MakeSceneTransitionAnimation(this);
                        StartActivity(intent, activityOptionsCompat.ToBundle());
                        break;
                    }
            }
            menuItem.SetChecked(false);
            _drawer?.CloseDrawer(GravityCompat.Start);

            return true;
        }

        protected override void OnStart()
        {
            base.OnStart();
            LogFileUtil.NewInstance(this).SaveLogToFile("开发日志：Ten's Blog安卓客户端OnStart()");
        }

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            RequestPermissionAsync();

            SetContentView(Resource.Layout.activity_main);

            InitToolbar();
            InitDrawer();
            InitNavigationView();
            InitRefreshLayout();
            InitRecyclerView();
            InitFab();
            InitSearchResultRecyclerView();
            InitSearchEditText();
            InitExitTransition();

            _coordinatorLayout = FindViewById<CoordinatorLayout>(Resource.Id.coordinator_main);

            await RssSubscribeAsync();
        }

        private void InitExitTransition()
        {
            if (Window != null)
            {
                Window.ExitTransition = TransitionInflater.From(this)?.InflateTransition(Resource.Transition.slide);
            }
        }

        /// <summary>
        ///     应用请求系统权限
        /// </summary>
        private async void RequestPermissionAsync()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Plugin.Permissions
                        .Abstractions.Permission.Storage))
                    {
                        SnackbarUtil.Show(this, _coordinatorLayout, "应用程序需要授予存储权限");
                    }

                    status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
                }

                if (status == PermissionStatus.Granted)
                {
                    // ShowToast("成功授予存储权限", ToastLength.Short);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    SnackbarUtil.Show(this, _coordinatorLayout, "您已拒绝授予存储权限");
                }
            }
            catch (Exception ex)
            {
                var message = $"请求授予存储权限发生错误: {ex.Message}";
                SnackbarUtil.Show(this, _coordinatorLayout, message);
                LogFileUtil.NewInstance(this).SaveLogToFile(message);
            }
        }

        /// <summary>
        ///     初始化搜索输入框
        /// </summary>
        private void InitSearchEditText()
        {
            _searchEditText = FindViewById<RemovableEditText>(Resource.Id.edit_search);
            if (_searchEditText != null)
                _searchEditText.TextChanged += (_, e) =>
                {
                    if (e.Text == null) return;
                    var text = e.Text.ToString();
                    _keywords = text;
                    var titles = new List<string>();
                    if (!string.IsNullOrWhiteSpace(text))
                        titles = (from entry in _entries
                                  where entry.Title.ToLower().Contains(text.ToLower())
                                  select entry.Title).Take(20).ToList();
                    _searchResultAdapter.RefreshItems(titles);
                };
        }

        /// <summary>
        ///     初始化搜索结果RecyclerView
        /// </summary>
        private void InitSearchResultRecyclerView()
        {
            _searchResultRecyclerView = FindViewById<RecyclerView>(Resource.Id.rv_search_result);
            if (_searchResultRecyclerView == null) return;
            _searchResultRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            _searchResultAdapter =
                new StandardRecyclerViewAdapter<string>(Android.Resource.Layout.SimpleListItem1,
                    new List<string>());
            _searchResultAdapter.OnGetConvertView += SearchResultAdapter_OnGetConvertView;
            _searchResultRecyclerView.SetAdapter(_searchResultAdapter);
        }

        /// <summary>
        ///     搜索结果RecyclerView适配器OnGetConvertView实现
        /// </summary>
        /// <param name="position"></param>
        /// <param name="parent"></param>
        /// <param name="item"></param>
        /// <param name="viewHolder"></param>
        /// <returns></returns>
        private View SearchResultAdapter_OnGetConvertView(int position, ViewGroup parent, string item,
            StandardRecyclerViewHolder viewHolder)
        {
            var textView = viewHolder.GetView<TextView>(Android.Resource.Id.Text1);
            textView!.SetHighLightText(this, item, _keywords,
                Resources?.GetColor(Resource.Color.colorAccent, null) ?? Color.Black);

            return viewHolder.GetConvertView();
        }

        /// <summary>
        ///     初始化RecyclerView
        /// </summary>
        private void InitRecyclerView()
        {
            _blogRecyclerView = FindViewById<RecyclerView>(Resource.Id.rv_main);
            _layoutManager = new LinearLayoutManager(this);
            if (_blogRecyclerView == null) return;
            _blogRecyclerView.SetLayoutManager(_layoutManager);

            _entries = new List<Entry>();

            _blogAdapter = new BlogRecyclerViewAdapter(this, _entries, Resource.Layout.item_blog);
            _animatorAdapter = new ScaleInAnimatorAdapter(_blogAdapter, _blogRecyclerView);
            _blogRecyclerView.SetAdapter(_animatorAdapter);

            _blogRecyclerView.AddOnScrollListener(new FabScrollListener(this));
        }

        /// <summary>
        ///     查询RSS订阅
        /// </summary>
        private async Task RssSubscribeAsync()
        {
            _dialogFragment = SimpleProgressDialogFragment.NewInstance("博文拼命加载中...");
            DialogShow();

            _entries = await RssSubscribeService.GetBlogEntries(this);
            _blogAdapter.RefreshItems(_entries);

            DialogDismiss();
        }

        /// <summary>
        ///     初始化SwipeRefreshLayout
        /// </summary>
        private void InitRefreshLayout()
        {
            _swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.srl_main);
            if (_swipeRefreshLayout != null)
                _swipeRefreshLayout.Refresh += async (_, _) =>
                {
                    await Task.Delay(1000);
                    await RssSubscribeAsync();
                    _swipeRefreshLayout.Refreshing = false;
                };
        }

        /// <summary>
        ///     初始化抽屉NavigationView
        /// </summary>
        private void InitNavigationView()
        {
            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView?.SetNavigationItemSelectedListener(this);
        }

        /// <summary>
        ///     初始化抽屉导航栏
        /// </summary>
        private void InitDrawer()
        {
            _drawer = FindViewById<Advance3DDrawerLayout>(Resource.Id.drawer_layout);
            var toggle = new ActionBarDrawerToggle(this, _drawer, _toolbar, Resource.String.navigation_drawer_open,
                Resource.String.navigation_drawer_close);
            if (_drawer == null) return;
            _drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            _drawer.SetViewScale(GravityCompat.Start, 0.96f);
            _drawer.SetViewElevation(GravityCompat.Start, 8f);
            _drawer.SetViewRotation(GravityCompat.Start, 15f);
        }

        /// <summary>
        ///     初始化工具栏
        /// </summary>
        private void InitToolbar()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_main);
            SetSupportActionBar(_toolbar);
        }

        /// <summary>
        ///     初始化FloatingActionButton
        /// </summary>
        private void InitFab()
        {
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            if (_fab != null) _fab.Click += (_, _) => { _blogRecyclerView.SmoothScrollToPosition(0); };
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (_drawer != null)
            {
                if (_drawer.IsDrawerOpen(GravityCompat.Start)) _drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_right_drawer:
                    {
                        if (_drawer != null)
                        {
                            _drawer.OpenDrawer(GravityCompat.End);
                            return true;
                        }

                        break;
                    }
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}