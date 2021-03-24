using System;
using System.Collections.Generic;

namespace TenBlogDroidApp.RssSubscriber.Models
{
    /// <summary>
    /// RSS入口(文章)模型
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// http链接
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 文章id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 文章发布日期时间
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        /// 文章更新日期时间
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// 文章模型
        /// </summary>
        public Summary Summary { get; set; }

        /// <summary>
        /// 文章分类清单
        /// </summary>
        public List<Category> Categories { get; set; }
    }
}
