using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Com.Devs.ReadMoreOptionLib;
using DE.Hdodenhof.CircleImageViewLib;
using Google.Android.Material.FloatingActionButton;
using Ten.Droid.Library.RecyclerView.Adapters;
using TenBlogDroidApp.DataAccess;
using TenBlogDroidApp.DataAccess.Entities;
using TenBlogDroidApp.Listeners;
using TenBlogDroidApp.Models;
using TenBlogDroidApp.Utils;
using Xamarin.Essentials;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "AppLog", Theme = "@style/AppTheme")]
    public class AppLogActivity : AppCompatActivity
    {
        private CanCheckedAdapter<AppLogViewModel> _adapter;
        private FloatingActionButton _fab;
        private List<AppLogViewModel> _logs;
        private RecyclerView _recyclerView;
        private Toolbar _toolbar;
        private ViewStates _isShowCheckbox = ViewStates.Gone;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_app_log);

            InitToolbar();
            InitRecyclerView();
            InitFab();
        }

        private void InitToolbar()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_app_log);
            if (_toolbar == null) return;
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        private void InitRecyclerView()
        {
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.rv_app_log);
            if (_recyclerView == null) return;
            _recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            _logs = new List<AppLogViewModel>();
            _adapter = new CanCheckedAdapter<AppLogViewModel>(Resource.Layout.item_app_log, _logs, Resource.Id.checkbox_app_log);
            _adapter.OnGetConvertView += Adapter_OnGetConvertView;
            _adapter.ItemLongClick += Adapter_ItemLongClick;
            _adapter.ItemChecked += Adapter_ItemChecked;
            _adapter.ItemUnChecked += Adapter_ItemUnChecked;
            _recyclerView.SetAdapter(_adapter);
        }

        private void Adapter_ItemLongClick(object sender, ItemClickEventArgs e)
        {
            if (_isShowCheckbox == ViewStates.Visible) return;
            _isShowCheckbox = ViewStates.Visible;
            _adapter.RefreshItems(_logs);
        }

        private void Adapter_ItemUnChecked(object sender, int e)
        {
            _logs[e].IsChecked = false;
        }

        private void Adapter_ItemChecked(object sender, int e)
        {
            _logs[e].IsChecked = true;
        }

        private View Adapter_OnGetConvertView(int position, ViewGroup parent, AppLogViewModel item, StandardHolder viewHolder)
        {
            var checkbox = viewHolder.GetView<AppCompatCheckBox>(Resource.Id.checkbox_app_log);
            if (checkbox != null) checkbox.Visibility = _isShowCheckbox;

            var ivLogType = viewHolder.GetView<CircleImageView>(Resource.Id.iv_app_log_picture);
            if (ivLogType != null)
            {
                switch (item.ApplicationLog.MessageType)
                {
                    case MessageType.Debug:
                        {
                            ivLogType.SetImageResource(Resource.Drawable.ic_adb_white_36dp);
                            ivLogType.SetColorFilter(Color.Rgb(33, 150, 243));
                            break;
                        }
                    case MessageType.Error:
                        {
                            ivLogType.SetImageResource(Resource.Drawable.ic_cancel_white_36dp);
                            ivLogType.SetColorFilter(Color.Rgb(244, 67, 54));
                            break;
                        }
                    case MessageType.Info:
                        {
                            ivLogType.SetImageResource(Resource.Drawable.ic_info_white_36dp);
                            ivLogType.SetColorFilter(Color.Rgb(76, 175, 80));
                            break;
                        }
                    case MessageType.Warning:
                        {
                            ivLogType.SetImageResource(Resource.Drawable.ic_warning_white_36dp);
                            ivLogType.SetColorFilter(Color.Rgb(255, 152, 0));
                            break;
                        }
                }
            }

            var tvMessageType = viewHolder.GetView<TextView>(Resource.Id.tv_log_msg_type);
            if (tvMessageType != null)
                tvMessageType.Text = $"[ID: {item.ApplicationLog.Id}]{item.ApplicationLog.MessageType}";

            var tvLogTime = viewHolder.GetView<TextView>(Resource.Id.tv_log_time);
            if (tvLogTime != null) tvLogTime.Text = $"{item.ApplicationLog.LogTime:yyyy-MM-dd HH:mm:ss}";

            //var tvLogContent = viewHolder.GetView<TextView>(Resource.Id.tv_log_msg_content);
            //if (Resources == null) return viewHolder.GetConvertView();
            //var readMoreOption = new ReadMoreOption.Builder(this)
            //    .TextLength(2, ReadMoreOption.TypeLine)
            //    .MoreLabel("展开")
            //    .LessLabel("收起")
            //    .MoreLabelColor(Resources.GetColor(Resource.Color.colorAccent, null))
            //    .LessLabelColor(Resources.GetColor(Resource.Color.colorAccent, null))
            //    .LabelUnderLine(true)
            //    .ExpandAnimation(true)
            //    .Build();
            //readMoreOption.AddReadMoreTo(tvLogContent, item.ApplicationLog.Message);

            return viewHolder.GetConvertView();
        }

        private void InitFab()
        {
            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab_app_log);
            if (_fab != null)
                _fab.Click += delegate
                {
                    var items = from log in _logs where log.IsChecked select log.ApplicationLog.Id;
                    var ids = string.Join(",", items);
                    SnackbarUtil.Show(this, _fab, $"当前选中项目ID: {ids}");
                };
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            var appLogs = TenBlogRepository.GetApplicationLogs();
            _logs = (from log in appLogs select new AppLogViewModel { ApplicationLog = log }).ToList();
            _adapter.RefreshItems(_logs);
        }
    }
}