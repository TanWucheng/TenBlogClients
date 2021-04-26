using System.Collections.Generic;
using System.Threading.Tasks;
using TenBlogCoreLib.Models;
using TenBlogCoreLib.RssSubscriber.Models;

namespace TenBlogCoreLib.RssSubscriber
{
    /// <summary>
    /// RSS订阅查询服务
    /// </summary>
    public static class RssSubscribeService
    {
        /// <summary>
        /// 订阅RSS，获取博客文章清单
        /// </summary>
        /// <param name="filePath">文件父级路径</param>
        /// <param name="articleCount">文章数量</param>
        /// <param name="doHttpRequest">是否执行HTTP请求获取最新订阅，如果是则用HTTP响应数据覆盖本地缓存XML，如果不是则直接读取本地缓存XML</param>
        public static Task<List<Entry>> GetBlogEntries(string filePath, int articleCount = int.MaxValue, bool doHttpRequest = false)
        {
            return Task.Run(async () =>
           {
               var feed = await Subscriber.Subscribe(Constants.BlogRssUrl, filePath, articleCount, doHttpRequest);
               return feed.Entries;
           });
        }
    }
}