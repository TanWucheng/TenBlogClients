using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using TenBlogDroidApp.DataAccess;
using TenBlogDroidApp.DataAccess.Entities;
using TenBlogDroidApp.Utils;

namespace TenBlogDroidApp.AppServices
{
    [Service(Exported = true, Name = "com.ten.tenblogdroidapp.CheckAppLifeService")]
    public class CheckAppLifeService : Service
    {
        private const string Tag = nameof(CheckAppLifeService);

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);
            Log.Debug(Tag, "Service Removed");
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(Tag, "Service Destroy");
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //ActivityManager activityManager = (ActivityManager)GetSystemService(ActivityService);
            //if (GetSystemService(ActivityService) is ActivityManager activityManager)
            //{
            //    var runningAppProcesses = activityManager.RunningAppProcesses;
            //    for (var i = 0; i < runningAppProcesses.Count; i++)
            //    {
            //        if (runningAppProcesses[i].ProcessName.Equals(PackageName))
            //        {
            //            Log.Debug(Tag, $"{runningAppProcesses[i].ProcessName}进程正在运行");
            //        }
            //    }
            //}
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            TenBlogRepository.SaveApplicationLog(new ApplicationLog
            {
                NameSpace = GetType().Namespace,
                ClassName = nameof(CheckAppLifeService),
                FuncName = "OnCreate()",
                LogTime = DateTime.Now,
                Message = $"{DateTime.Now:yyyy-MM-dd}-Application Start.[岱宗夫如何，齐鲁青未了。造化钟神秀，阴阳割昏晓。荡胸生层云，决眦入归鸟。会当凌绝顶，一览众山小。]",
                MessageType = MessageType.Debug
            });
            ToastUtil.Show(this, "Application启动");
        }
    }
}
