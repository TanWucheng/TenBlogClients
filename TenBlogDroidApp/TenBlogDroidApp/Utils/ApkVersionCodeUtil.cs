using System;
using Android.Content;
using Ten.Droid.Library.Utils;

namespace TenBlogDroidApp.Utils
{
    public class ApkVersionCodeUtil
    {
        /// <summary>
        /// 获取当前app version code
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetVersionCode(Context context)
        {
            int versionCode = -1;
            try
            {
                versionCode = (int)context.ApplicationContext.PackageManager.GetPackageInfo(context.PackageName, 0).LongVersionCode;
            }
            catch (Exception ex)
            {
                LogFileUtil.NewInstance(context).SaveLogToFile($"ApkVersionCodeUtil.GetVersionCode()发生异常: {ex.Message}");
            }
            return versionCode;
        }

        /// <summary>
        /// 获取当前app version name
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetVersionName(Context context)
        {
            String versionName = "";
            try
            {
                versionName = context.ApplicationContext.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            }
            catch (Exception ex)
            {
                LogFileUtil.NewInstance(context).SaveLogToFile($"ApkVersionCodeUtil.GetVersionName()发生异常: {ex.Message}");
            }
            return versionName;
        }
    }
}
