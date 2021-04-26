namespace TenBlogCoreLib.RssSubscriber.Models
{
    /// <summary>
    /// 文章内容模型
    /// </summary>
    public class Summary
    {
        /// <summary>
        /// 文章类别
        /// </summary>
        public SummaryType SummaryType { get; set; }

        /// <summary>
        /// 文章内容
        /// </summary>
        public string Content { get; set; }
    }
}
