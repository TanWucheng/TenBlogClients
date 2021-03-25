namespace TenBlogNet.AndroidApp.RssSubscriber.Models
{
    /// <summary>
    /// 文章分类
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 分类名称
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// 分类Http链接
        /// </summary>
        public string Scheme { get; set; }
    }
}
