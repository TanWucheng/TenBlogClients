using Android.Webkit;

namespace TenBlogDroidApp.Utils
{
    public class SimpleWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            if (request.Url != null) view.LoadUrl(request.Url.ToString()!);
            return false;
        }
    }
}