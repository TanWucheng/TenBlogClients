using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using Xamarin.Essentials;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "Who am i?")]
    public class WhoAmIActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_whoami);

        }
    }
}
