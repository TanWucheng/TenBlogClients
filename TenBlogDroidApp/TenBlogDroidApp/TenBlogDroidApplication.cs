using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using TenBlogDroidApp.AppServices;

namespace TenBlogDroidApp
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class TenBlogDroidApplication : Application
    {
        public TenBlogDroidApplication(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            var intent = new Intent(this, typeof(CheckAppLifeService));
            intent.SetFlags(ActivityFlags.NewTask);
            ApplicationContext.StartService(intent);
        }
    }
}
