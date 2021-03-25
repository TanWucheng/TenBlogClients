using Android.Content;
using Ten.Droid.Library.Utils;
using Volley;

namespace TenBlogNet.AndroidApp.RssSubscriber.ImageGetter
{
    internal class VolleyResponseErrorListener : Java.Lang.Object, Response.IErrorListener
    {
        private readonly Context _context;

        public VolleyResponseErrorListener(Context context)
        {
            _context = context;
        }

        public void OnErrorResponse(VolleyError p0)
        {
            LogFileUtil.NewInstance(_context).SaveLogToFile($"VolleyResponseErrorListener: {p0.Message}");
        }
    }
}