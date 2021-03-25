using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace TenBlogNet.AndroidApp.Fragments
{
    public class SimpleProgressDialogFragment : DialogFragment
    {
        private readonly string _strHint;
        private static SimpleProgressDialogFragment _progressDialogFragment;
        private View _rootView;

        private SimpleProgressDialogFragment(string strHint)
        {
            _strHint = strHint;
        }

        public static SimpleProgressDialogFragment NewInstance(string strHint)
        {
            return _progressDialogFragment ??= new SimpleProgressDialogFragment(strHint);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window?.RequestFeature(WindowFeatures.NoTitle);
            _rootView = base.OnCreateView(inflater, container, savedInstanceState);
            if (_rootView != null) return _rootView;
            _rootView = LayoutInflater.Inflate(Resource.Layout.simple_progress_dialog, null);
            var textView = _rootView?.FindViewById<TextView>(Resource.Id.tv_dialog_progress_hint);
            if (textView != null)
            {
                textView.Text = _strHint;
            }

            return _rootView;
        }
    }
}