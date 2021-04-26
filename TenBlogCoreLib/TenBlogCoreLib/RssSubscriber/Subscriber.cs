using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TenBlogCoreLib.RssSubscriber.Models;
using TenBlogCoreLib.Utils;

namespace TenBlogCoreLib.RssSubscriber
{
    /// <summary>
    ///     RSS订阅器
    /// </summary>
    public static class Subscriber
    {
        private const string RssXmlFileName = "atom.xml";

        /// <summary>
        ///     订阅RSS
        /// </summary>
        /// <param name="rssUrl">RSS订阅源URL，文件系统路径或者HTTP链接</param>
        /// <param name="filePath">文件父级路径</param>
        /// <param name="articleCount">文章数量</param>
        /// <param name="doHttpRequest">是否执行HTTP请求获取最新订阅，如果是则用HTTP响应数据覆盖本地缓存XML，如果不是则直接读取本地缓存XML</param>
        /// <returns></returns>
        public static async Task<Feed> Subscribe(string rssUrl, string filePath, int articleCount = int.MaxValue,
            bool doHttpRequest = false)
        {
            var xmlDoc = new XmlDocument();
            if (string.IsNullOrWhiteSpace(rssUrl)) return null;
            xmlDoc.Load(rssUrl);

            if (doHttpRequest)
            {
                var xml = await HttpRequestRssFeedAsync(rssUrl);
                xmlDoc.LoadXml(xml);
                try
                {
                    WriteCacheRssXmlAsync(filePath, xml);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                var xml = await ReadCacheRssXmlAsync(filePath);
                if (string.IsNullOrWhiteSpace(xml))
                {
                    xml = await HttpRequestRssFeedAsync(rssUrl);
                    try
                    {
                        WriteCacheRssXmlAsync(filePath, xml);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                xmlDoc.LoadXml(xml);
            }

            if (!xmlDoc.HasChildNodes) return null;
            //var entryNodeList = xmlDoc.GetElementsByTagName("entry");
            var childList = xmlDoc.ChildNodes;
            foreach (XmlNode child in childList)
                switch (child.Name.ToLower())
                {
                    case "feed":
                    {
                        return ReadFeed(child, articleCount);
                    }
                }

            return null;
        }

        /// <summary>
        ///     读取本地RSS订阅XML缓存文件
        /// </summary>
        /// <param name="filePath">文件父级路径</param>
        /// <returns></returns>
        private static async Task<string> ReadCacheRssXmlAsync(string filePath)
        {
            var absFilePath = Path.Combine(filePath ?? string.Empty, RssXmlFileName);
            if (!File.Exists(absFilePath)) return string.Empty;

            var xml = await File.ReadAllTextAsync(absFilePath, Encoding.UTF8);
            return xml;
        }

        /// <summary>
        ///     保存RSS订阅XML为本地文件
        /// </summary>
        /// <param name="filePath">文件父级路径</param>
        /// <param name="xml">XML内容</param>
        private static async void WriteCacheRssXmlAsync(string filePath, string xml)
        {
            var absFilePath = Path.Combine(filePath ?? string.Empty, RssXmlFileName);
            if (File.Exists(absFilePath)) File.Delete(absFilePath);

            await using var stream = new FileStream(absFilePath, FileMode.Create, FileAccess.Write);
            var buffer = Encoding.UTF8.GetBytes(xml);
            await stream.WriteAsync(buffer);
        }

        /// <summary>
        ///     执行HTTP请求获取RSS订阅XML
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task<string> HttpRequestRssFeedAsync(string url)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            return response.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        ///     读取RSS订阅信息XML节点
        /// </summary>
        /// <param name="feedNode">RSS Feed节点</param>
        /// <param name="articleCount">文章数量</param>
        /// <returns></returns>
        private static Feed ReadFeed(XmlNode feedNode, int articleCount)
        {
            var feed = new Feed();
            if (!feedNode.HasChildNodes) return feed;
            var feedChildNodes = feedNode.ChildNodes;
            var entryChildNodes = new List<XmlNode>();
            var authors = new List<Author>();
            foreach (XmlNode feedChildNode in feedChildNodes)
                switch (feedChildNode.Name.ToLower())
                {
                    case "title":
                    {
                        feed.Title = feedChildNode.InnerText;
                        break;
                    }
                    case "icon":
                    {
                        feed.Icon = feedChildNode.InnerText;
                        break;
                    }
                    case "subtitle":
                    {
                        feed.Subtitle = feedChildNode.InnerText;
                        break;
                    }
                    case "link":
                    {
                        if (feedChildNode.Attributes != null)
                        {
                            if (feedChildNode.Attributes["rel"] != null &&
                                feedChildNode.Attributes["rel"].Value == "self")
                                feed.SelfLink = feedChildNode.Attributes["href"]?.Value;
                            else
                                feed.Link = feedChildNode.Attributes["href"]?.Value;
                        }

                        break;
                    }
                    case "id":
                    {
                        feed.Id = feedChildNode.InnerText;
                        break;
                    }
                    case "author":
                    {
                        authors.Add(new Author
                        {
                            Name = feedChildNode.InnerText
                        });
                        break;
                    }
                    case "entry":
                    {
                        entryChildNodes.Add(feedChildNode);
                        break;
                    }
                }

            var entries = ReadEntries(entryChildNodes, articleCount);
            feed.Entries = entries;
            feed.Author = authors.Any() ? authors[0] : new Author();

            return feed;
        }

        /// <summary>
        ///     读取文章信息
        /// </summary>
        /// <param name="entryNodes">文章节点清单</param>
        /// <param name="articleCount">文章数量</param>
        /// <returns></returns>
        private static List<Entry> ReadEntries(List<XmlNode> entryNodes, int articleCount)
        {
            try
            {
                var count = 1;
                var entries = new List<Entry>();
                foreach (var entryNode in entryNodes)
                {
                    var entry = new Entry();
                    if (entryNode.HasChildNodes)
                    {
                        var entryChildNodes = entryNode.ChildNodes;
                        var categories = new List<Category>();
                        foreach (XmlNode entryChildNode in entryChildNodes)
                            switch (entryChildNode.Name.ToLower())
                            {
                                case "title":
                                {
                                    entry.Title = entryChildNode.InnerText;
                                    break;
                                }
                                case "link":
                                {
                                    if (entryChildNode.Attributes != null)
                                    {
                                        var href = entryChildNode.Attributes["href"]?.Value;
                                        entry.Link = href;
                                    }

                                    break;
                                }
                                case "id":
                                {
                                    entry.Id = entryChildNode.InnerText;
                                    break;
                                }
                                case "published":
                                {
                                    if (DateTime.TryParse(entryChildNode.InnerText, out var published))
                                        entry.Published = published;
                                    break;
                                }
                                case "updated":
                                {
                                    if (DateTime.TryParse(entryChildNode.InnerText, out var updated))
                                        entry.Updated = updated;
                                    break;
                                }
                                case "summary":
                                {
                                    var summaryType = SummaryType.Plain;
                                    if (entryChildNode.Attributes != null)
                                    {
                                        var type = entryChildNode.Attributes["type"]?.Value;
                                        summaryType =
                                            EnumParser<SummaryType>.Parse(type!, SummaryType.Plain);
                                    }

                                    var summary = new Summary
                                    {
                                        Content = entryChildNode.InnerText,
                                        SummaryType = summaryType
                                    };
                                    entry.Summary = summary;
                                    break;
                                }
                                case "category":
                                {
                                    if (entryChildNode.Attributes != null)
                                    {
                                        var term = entryChildNode.Attributes["term"]?.Value;
                                        var scheme = entryChildNode.Attributes["scheme"]?.Value;
                                        var category = new Category
                                        {
                                            Scheme = scheme,
                                            Term = term
                                        };
                                        categories.Add(category);
                                    }

                                    break;
                                }
                            }

                        entry.Categories = categories;
                        entries.Add(entry);
                    }

                    count += 1;
                    if (count > articleCount) break;
                }

                return entries;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}