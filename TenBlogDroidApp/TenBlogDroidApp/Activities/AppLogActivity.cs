using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using Ten.Droid.Library.RecyclerView.Adapters;
using TenBlogDroidApp.DataAccess;
using TenBlogDroidApp.Models;
using TenBlogDroidApp.Utils;
using Xamarin.Essentials;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "About", Theme = "@style/AppTheme")]
    public class AppLogActivity : AppCompatActivity
    {
        private StandardRecyclerViewAdapter<AppLogViewModel> _adapter;
        private FloatingActionButton _fab;
        private List<AppLogViewModel> _logs;
        private RecyclerView _recyclerView;
        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_app_log);

            InitToolbar();
            InitRecyclerView();

            _fab = FindViewById<FloatingActionButton>(Resource.Id.fab_app_log);
            if (_fab != null)
                _fab.Click += delegate
                {
                    var items = from log in _logs where log.Checked select log.ApplicationLog.Id;
                    var ids = string.Join(",", items);
                    SnackbarUtil.Show(this, _fab, $"当前选中项目ID: {ids}");
                };
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
            _adapter = new StandardRecyclerViewAdapter<AppLogViewModel>(
                Android.Resource.Layout.SimpleListItemMultipleChoice, _logs);
            _adapter.OnGetConvertView += Adapter_OnGetConvertView;
            _recyclerView.SetAdapter(_adapter);
        }

        private static View Adapter_OnGetConvertView(int position, ViewGroup parent, AppLogViewModel item,
            StandardRecyclerViewHolder viewHolder)
        {
            var itemView = viewHolder.GetConvertView();
            if (itemView is not CheckedTextView textView) return itemView;
            textView.Text =
                $"{item.ApplicationLog.Id}. [{item.ApplicationLog.LogTime:yyyy-MM-dd HH:mm:ss}]{item.ApplicationLog.FuncName}: {item.ApplicationLog.Message}";
            textView.Checked = item.Checked;
            textView.Click += delegate
            {
                textView.Checked = !textView.Checked;
                item.Checked = textView.Checked;
            };

            return itemView;
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
            _logs = (from log in appLogs select new AppLogViewModel {ApplicationLog = log, Checked = false}).ToList();
            _adapter.RefreshItems(_logs);
        }
    }
}