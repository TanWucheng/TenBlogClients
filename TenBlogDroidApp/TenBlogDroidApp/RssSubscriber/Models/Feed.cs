using System;
using System.Collections.Generic;

namespace TenBlogDroidApp.RssSubscriber.Models
{
    /// <summary>
    /// Rss订阅源的模型
    /// </summary>
    public class Feed
    {
        /// <summary>
        /// 订阅源的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 订阅源的图标url
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 订阅源副标题
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// 订阅源XML本身的链接
        /// </summary>
        public string SelfLink { get; set; }

        /// <summary>
        /// 网站源地址
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 订阅源XML更新日期时间
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// 订阅源id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// RSS订阅源所有者信息
        /// </summary>
        public Author Author { get; set; }

        /// <summary>
        /// RSS入口(文章)清单
        /// </summary>
        public List<Entry> Entries { get; set; }
    }
}
