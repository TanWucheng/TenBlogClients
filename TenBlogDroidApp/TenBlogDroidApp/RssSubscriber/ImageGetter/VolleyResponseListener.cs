using Android.Graphics;
using Volley;

namespace TenBlogDroidApp.RssSubscriber.ImageGetter
{
    internal class VolleyResponseListener : Java.Lang.Object, Response.IListener
    {
        private readonly string _fileName;
        private readonly HtmlImageGetter _imageGetter;

        public VolleyResponseListener(string fileName, HtmlImageGetter imageGetter)
        {
            _fileName = fileName;
            _imageGetter = imageGetter;
        }

        public void OnResponse(Java.Lang.Object p0)
        {
            if (p0 is Bitmap bitmap)
            {
                _imageGetter.SaveBitmap(_fileName, bitmap);
            }
        }
    }
}