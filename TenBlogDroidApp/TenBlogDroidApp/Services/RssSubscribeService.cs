using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using TenBlogDroidApp.RssSubscriber;
using TenBlogDroidApp.RssSubscriber.Models;

namespace TenBlogDroidApp.Services
{
    internal static class RssSubscribeService
    {
        private const string RssUrl = "https://tanwucheng.github.io/atom.xml";

        /// <summary>
        /// 订阅RSS，获取博客文章清单
        /// </summary>
        /// <param name="context">Activity上下文</param>
        /// <param name="articleCount">文章数量</param>
        /// <param name="doHttpRequest">是否执行HTTP请求获取最新订阅，如果是则用HTTP响应数据覆盖本地缓存XML，如果不是则直接读取本地缓存XML</param>
        public static Task<List<Entry>> GetBlogEntries(Context context, int articleCount = int.MaxValue, bool doHttpRequest = false)
        {
            return Task.Run(async () =>
           {
               var feed = await Subscriber.Subscribe(RssUrl, context, articleCount, doHttpRequest);
               return feed.Entries;
           });
        }
    }
}