using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Transitions;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Ten.Droid.Library.Extensions;
using TenBlogDroidApp.Utils;
using Xamarin.Essentials;
using File = Java.IO.File;
using FileProvider = AndroidX.Core.Content.FileProvider;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "ContactFeedback", Theme = "@style/AppTheme.NoActionBar")]
    public class ContactFeedbackActivity : AppCompatActivity
    {
        private MaterialButton _contactButton;
        private MaterialButton _feedbackButton;
        private FloatingActionButton _githubIssuesButton;
        private TextView _statementTextView;
        private Toolbar _toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_contact_feedback);

            InitEnterTransition();

            InitToolbar();
            InitStatement();
            InitContactButton();
            InitFeedbackButton();
            InitIssuesButton();
        }

        private void InitEnterTransition()
        {
            if (Window == null) return;
            var slide = new Slide();
            slide.SetDuration(500);
            slide.SlideEdge = GravityFlags.Start;
            Window.EnterTransition = slide;
            Window.ReenterTransition = new Explode().SetDuration(600);
        }

        private void InitToolbar()
        {
            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_contact_feedback);
            if (_toolbar == null) return;
            if (Resources != null) _toolbar.Title = Resources.GetString(Resource.String.contact_feedback);
            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        private void InitStatement()
        {
            _statementTextView = FindViewById<TextView>(Resource.Id.tv_feedback_statement);
            _statementTextView?.SetHtml(Constants.FeedbackStatement);
        }

        private void InitContactButton()
        {
            _contactButton = FindViewById<MaterialButton>(Resource.Id.button_contact);
            if (_contactButton == null) return;
            _contactButton.Click += delegate
            {
                Intent intent = new(Intent.ActionSend);
                intent.SetData(Uri.Parse(Constants.MailToMe));
                intent.PutExtra(Intent.ExtraSubject, Constants.ContactSubjectExample);
                intent.PutExtra(Intent.ExtraText,
                    string.Format(Constants.ContactBodyExample, DateTime.Now.ToLongDateString(),
                        DateTime.Now.ToLongTimeString()));
                StartActivityForResult(intent, RequestCodes.SendEmail);
            };
        }

        private void InitFeedbackButton()
        {
            _feedbackButton = FindViewById<MaterialButton>(Resource.Id.button_feedback);
            if (_feedbackButton == null) return;
            _feedbackButton.Click += delegate
            {
                Intent intent = new(Intent.ActionSend);
                //intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                intent.SetData(Uri.Parse(Constants.MailToMe));
                intent.PutExtra(Intent.ExtraSubject, Constants.FeedbackSubjectExample);
                var appDocPath = FilesDir?.AbsolutePath;
                var absFilePath = Path.Combine(appDocPath ?? string.Empty, $"applog_{DateTime.Now:yyyyMMdd}.log");
                var logUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", new File(absFilePath));
                //GrantUriPermission("com.microsoft.office.outlook", logUri, ActivityFlags.GrantReadUriPermission);
                intent.PutExtra(Intent.ExtraText,
                    string.Format(Constants.FeedbackBodyExample, DateTime.Now.ToLongDateString(),
                        DateTime.Now.ToLongTimeString()));
                intent.PutExtra(Intent.ExtraStream, logUri);
                StartActivityForResult(intent, RequestCodes.SendEmail);
            };
        }

        private void InitIssuesButton()
        {
            _githubIssuesButton = FindViewById<FloatingActionButton>(Resource.Id.fab_github_issues);
            if (_githubIssuesButton != null)
                _githubIssuesButton.Click += delegate
                {
                    Intent intent = new(Intent.ActionView);
                    intent.SetData(Uri.Parse(Constants.IssuesUrl));
                    StartActivity(intent);
                };
        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            var layoutParams = (CoordinatorLayout.LayoutParams)_githubIssuesButton.LayoutParameters;
            if (layoutParams == null) return;
            layoutParams.AnchorId = -1;
            _githubIssuesButton.LayoutParameters = layoutParams;

            //IInterpolator interpolator = new FastOutSlowInInterpolator();
            //ViewCompat.Animate(_githubIssuesButton)
            //    .ScaleX(0.0F)
            //    .ScaleY(0.0F)
            //    .Alpha(0.0F)
            //    .SetInterpolator(interpolator)
            //    .WithLayer()
            //    .Start();
        }
    }
}