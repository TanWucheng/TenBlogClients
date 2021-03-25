using System;
using Android.App;
using Android.Content;
using Android.OS;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace TenBlogNet.AndroidApp.Widgets
{
    public class SimpleDialogFragment : DialogFragment
    {
        private readonly Context _context;
        private readonly string _message;
        private bool _isShowPositiveBtn;
        private bool _isShowNegativeBtn;
        private string _positiveText;
        private string _negativeText;
        private EventHandler<DialogClickEventArgs> _positiveHandler;
        private EventHandler<DialogClickEventArgs> _negativeHandler;

        public SimpleDialogFragment(Context context, string message, string positiveText = "OK", string negativeText = "Cancel", bool isShowPositiveBtn = false, bool isShowNegativeBtn = false,
           EventHandler<DialogClickEventArgs> positiveHandler = null, EventHandler<DialogClickEventArgs> negativeHandler = null)
        {
            _context = context;
            _message = message;
            _isShowPositiveBtn = isShowPositiveBtn;
            _isShowNegativeBtn = isShowNegativeBtn;
            _positiveText = positiveText;
            _negativeText = negativeText;
            _positiveHandler = positiveHandler;
            _negativeHandler = negativeHandler;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(_context);
            builder.SetMessage(_message);
            if (_isShowPositiveBtn)
            {
                builder.SetPositiveButton(_positiveText, _positiveHandler);
            }
            return base.OnCreateDialog(savedInstanceState);
        }
    }
}