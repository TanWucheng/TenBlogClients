namespace TenBlogDroidApp.Utils
{
    internal struct Constants
    {
        /// <summary>
        ///     新浪App Key
        /// </summary>
        public const string SinaAppKey = "2553636678";

        /// <summary>
        ///     博客网站地址
        /// </summary>
        public const string BlogUrl = "https://tanwucheng.github.io";

        /// <summary>
        ///     Github问题反馈URL
        /// </summary>
        public const string IssuesUrl = "https://github.com/TanWucheng/TenBlogClients/issues";

        /// <summary>
        ///     邮件发送URL
        /// </summary>
        public const string MailToMe = "mailto:tanwucheng@outlook.com";

        /// <summary>
        ///     应用程序声明
        /// </summary>
        public const string FeedbackStatement =
            @"<h1>说明</h1>
            <ol>
            <li>关于应用程序的使用的问题、不足之处、改进建议等请点击下方的<code>联系作者</code>按钮，发送邮件向作者反馈</li>
            <li>在应用程序的使用过程中的发生的错误、崩溃等异常情况会被记录在日志文件里，请点击下方的<code>错误反馈</code>按钮，发送邮件向作者反馈问题</li>
            </ol>";

        /// <summary>
        ///     错误反馈邮件示例标题
        /// </summary>
        public const string FeedbackSubjectExample = "示例标题: TenBlog安卓客户端错误日志反馈";

        /// <summary>
        ///     错误反馈邮件示例内容
        ///     占位符0、1分别填充当前日期和当前时间
        /// </summary>
        public const string FeedbackBodyExample =
            @"<h2 style='color:#ff5722;'>App错误日志反馈</h2>
            <p style='color:#00bcd4;'>注：邮件附件默认添加的最新一份App错误日志，如果您有其他问题反馈，请在正文里补充。</p>
            <p style='color:#2196F3;'>{0} {1}</p>";

        /// <summary>
        ///     联系作者邮件示例标题
        /// </summary>
        public const string ContactSubjectExample = "示例标题: TenBlog安卓客户端改进建议";

        /// <summary>
        ///     联系作者邮件示例内容
        ///     占位符0、1分别填充当前日期和当前时间
        /// </summary>
        public const string ContactBodyExample =
            @"<h2 style='color:#ff5722;'>App改进建议</h2>
            <p style='color:#00bcd4;'>注：欢迎向我提出不足之处和改进建议</p>
            <p style='color:#2196F3;'>{0} {1}</p>";
    }
}