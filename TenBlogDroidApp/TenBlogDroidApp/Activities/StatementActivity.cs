using Android.App;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Xamarin.Essentials;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "About")]
    public class StatementActivity : AppCompatActivity
    {
        private Toolbar _toolbar;
        private string _shareTitle;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            _shareTitle = Intent.GetStringExtra("share_title");

            SetContentView(Resource.Layout.activity_statement);

            InitToolbar();
        }

        private void InitToolbar()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_statement);
            if (_toolbar == null) return;
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }
    }
}
