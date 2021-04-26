using System;
using System.IO;
using System.Reflection;

namespace TenBlogNet.WpfApp.Domain
{
    /// <summary>
    ///     程序集工具，参考https://www.cnblogs.com/jiutianxingchen/archive/2013/01/29/2881695.html
    /// </summary>
    public class AssemblyUtil
    {
        /// <summary>
        ///     获取程序集项目属性内容
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetAssemblyAttr(Type type)
        {
            // 程序集版本号，要用这个方法获取，无法用下边的方法获取，原因不知
            if (type.ToString() == "System.Reflection.AssemblyVersionAttribute")
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                if (version is not null)
                    return version.ToString();
            }

            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(type, false);
            if (attributes.Length <= 0)
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            switch (type.ToString())
            {
                case "System.Reflection.AssemblyCompanyAttribute":
                {
                    // 公司
                    var company = (AssemblyCompanyAttribute) attributes[0];
                    if (company.Company != "") return company.Company;
                    break;
                }
                case "System.Reflection.AssemblyCopyrightAttribute":
                {
                    // 版权
                    var company = (AssemblyCopyrightAttribute) attributes[0];
                    if (company.Copyright != "") return company.Copyright;
                    break;
                }
                case "System.Reflection.AssemblyTitleAttribute":
                {
                    // 标题
                    var company = (AssemblyTitleAttribute) attributes[0];
                    if (company.Title != "") return company.Title;
                    break;
                }
                case "System.Reflection.AssemblyDescriptionAttribute":
                {
                    // 备注
                    var company = (AssemblyDescriptionAttribute) attributes[0];
                    if (company.Description != "") return company.Description;
                    break;
                }
                case "System.Reflection.AssemblyProductAttribute":
                {
                    // 产品名称
                    var company = (AssemblyProductAttribute) attributes[0];
                    if (company.Product != "") return company.Product;
                    break;
                }
                case "System.Reflection.AssemblyTrademarkAttribute":
                {
                    // 商标
                    var company = (AssemblyTrademarkAttribute) attributes[0];
                    if (company.Trademark != "") return company.Trademark;
                    break;
                }
                case "System.Reflection.AssemblyConfigurationAttribute":
                {
                    // 获取程序集配置信息，具体什么内容，不清楚
                    var company = (AssemblyConfigurationAttribute) attributes[0];
                    if (company.Configuration != "") return company.Configuration;
                    break;
                }
                case "System.Reflection.AssemblyCultureAttribute":
                {
                    // 获取属性化程序集支持的区域性，具体什么内容，不清楚
                    var company = (AssemblyCultureAttribute) attributes[0];
                    if (company.Culture != "") return company.Culture;
                    break;
                }
                case "System.Reflection.AssemblyVersionAttribute":
                {
                    // 程序集版本号
                    var company = (AssemblyVersionAttribute) attributes[0];
                    if (company.Version != "") return company.Version;
                    break;
                }
                case "System.Reflection.AssemblyFileVersionAttribute":
                {
                    // 文件版本号
                    var company = (AssemblyFileVersionAttribute) attributes[0];
                    if (company.Version != "") return company.Version;
                    break;
                }
                case "System.Reflection.AssemblyInformationalVersionAttribute":
                {
                    // 产品版本号
                    var company = (AssemblyInformationalVersionAttribute) attributes[0];
                    if (company.InformationalVersion != "") return company.InformationalVersion;
                    break;
                }
            }

            // 如果没有  属性，或者  属性为一个空字符串，则返回 .exe 的名称 
            return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
        }
    }
}