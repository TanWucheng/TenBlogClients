using Com.Tencent.Smtt.Export.External.Interfaces;
using Com.Tencent.Smtt.Sdk;

namespace TenBlogDroidApp.Utils
{
    public class SimpleWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView p0, IWebResourceRequest p1)
        {
            if (p1.Url != null) p0.LoadUrl(p1.Url.ToString()!);
            return true;
        }
    }
}