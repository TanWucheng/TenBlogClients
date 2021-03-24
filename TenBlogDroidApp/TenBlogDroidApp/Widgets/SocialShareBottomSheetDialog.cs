using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using Com.Sina.Weibo.Sdk;
using Com.Sina.Weibo.Sdk.Api;
using Com.Sina.Weibo.Sdk.Auth;
using Com.Sina.Weibo.Sdk.Share;
using Google.Android.Material.BottomSheet;
using Ten.Droid.Library.Utils;
using TenBlogDroidApp.Utils;
using Uri = Android.Net.Uri;

namespace TenBlogDroidApp.Widgets
{
    public class SocialShareBottomSheetDialog : BottomSheetDialog, IWbShareCallback
    {
        private const string Tag = nameof(SocialShareBottomSheetDialog);

        private WbShareHandler _shareHandler;
        private readonly AppCompatActivity _activity;

        public SocialShareBottomSheetDialog(AppCompatActivity activity) : base(activity)
        {
            _activity = activity;
            Init(activity);
            InitWeiboSdk();
        }

        private void InitWeiboSdk()
        {
            // 创建微博API接口类对象
            WbSdk.Install(Context, new AuthInfo(Context, Constants.SinaAppKey, Constants.BlogUrl, string.Empty));
            _shareHandler = new WbShareHandler(_activity);
            _shareHandler.RegisterApp();
        }

        public void OnWbShareCancel()
        {
            Log.Debug(Tag, "微博分享取消");
        }

        public void OnWbShareFail()
        {
            Log.Debug(Tag, "微博分享取消");
        }

        public void OnWbShareSuccess()
        {
            Log.Debug(Tag, "微博分享成功");
        }

        private void Init(Context context)
        {
            var contentView = LayoutInflater.From(context)
                                ?.Inflate(Resource.Layout.content_social_shara_sheet, null);
            InitBottomShareMenu(context, contentView);

            Window?.AddFlags(WindowManagerFlags.TranslucentStatus);
            SetContentView(contentView!);
            SetCancelable(true);
            SetCanceledOnTouchOutside(true);
        }

        private void InitBottomShareMenu(Context context, View contentView)
        {
            var sysShareMenu = contentView.FindViewById(Resource.Id.linear_sys_share);
            if (sysShareMenu != null)
            {
                sysShareMenu.Click += delegate
                {
                    var intent = new Intent(Intent.ActionSend);
                    intent.SetType("text/plain");
                    intent.PutExtra(Intent.ExtraTitle, "分享博客网址");
                    intent.PutExtra(Intent.ExtraText, Constants.BlogUrl);
                    context.StartActivity(Intent.CreateChooser(intent, "分享到"));
                    Dismiss();
                };
            }

            var copyUrlMenu = contentView.FindViewById(Resource.Id.linear_copy_url);
            if (copyUrlMenu != null)
            {
                copyUrlMenu.Click += delegate
                {
                    if (context.GetSystemService(Context.ClipboardService) is not ClipboardManager manager) return;
                    var data = ClipData.NewPlainText("shareUrl",
                        "https://tanwucheng.github.io");
                    manager.PrimaryClip = data;
                    SnackbarUtil.Show(_activity, copyUrlMenu, "链接已复制到剪贴板");

                    Dismiss();
                };
            }

            var smsShareMenu = contentView.FindViewById(Resource.Id.linear_sms_share);
            if (smsShareMenu != null)
            {
                smsShareMenu.Click += delegate
                {
                    var uri = Uri.Parse("smsto:");
                    var intent = new Intent(Intent.ActionSendto, uri);
                    intent.PutExtra("sms_body", $"来自Ten's Blog的分享短信，欢迎访问的我的博客网站：{Constants.BlogUrl}");
                    context.StartActivity(intent);

                    Dismiss();
                };
            }

            var emailShareMenu = contentView.FindViewById(Resource.Id.linear_email_share);
            if (emailShareMenu != null)
            {
                emailShareMenu.Click += delegate
                {
                    var intent = new Intent(Intent.ActionSend);
                    intent.SetData(Uri.Parse("mailto:example@example.com"));
                    intent.PutExtra(Intent.ExtraSubject, "欢迎访问我的博客网站");
                    intent.PutExtra(Intent.ExtraText, $"<h5>来自Ten's Blog的分享邮件</h5><p><a href='{Constants.BlogUrl}'>点此</a>访问博客网站</p>");
                    context.StartActivity(Intent.CreateChooser(intent, "选择邮箱客户端"));

                    Dismiss();
                };
            }

            var weChatMenu = contentView.FindViewById(Resource.Id.linear_wechat_share);
            if (weChatMenu != null)
            {
                weChatMenu.Click += delegate
                {
                    if (PlatformUtil.IsInstallApp(context, PlatformUtil.WeChatPackage))
                    {
                        Intent intent = new();
                        ComponentName cop = new(PlatformUtil.WeChatPackage, "com.tencent.mm.ui.tools.ShareImgUI");
                        intent.SetComponent(cop);
                        intent.SetAction(Intent.ActionSend);
                        intent.PutExtra("android.intent.extra.TEXT", "https://tanwucheng.github.io");
                        intent.PutExtra("Kdescription", "Ten's Blog网站地址");
                        intent.SetFlags(ActivityFlags.NewTask);
                        context.StartActivity(intent);

                        Dismiss();
                    }
                    else
                    {
                        SnackbarUtil.Show(_activity, contentView, "您需要安装微信客户端");
                    }
                };
            }

            var weiboMenu = contentView.FindViewById(Resource.Id.linear_weibo_share);
            if (weiboMenu != null)
            {
                weiboMenu.Click += delegate
                 {
                     if (PlatformUtil.IsInstallApp(context, PlatformUtil.WeiboPackage))
                     {
                         WeiboMultiMessage weiboMessage = new() { ImageObject = GetImageObj(_activity) };
                         if (_activity.Resources != null)
                             weiboMessage.TextObject = GetTextObj(_activity.Resources.GetString(Resource.String.app_name_cn), "来自Ten's Blog的分享");
                         _shareHandler.ShareMessage(weiboMessage, false);

                         Dismiss();
                     }
                     else
                     {
                         SnackbarUtil.Show(_activity, contentView, "您需要安装微博客户端");
                     }
                 };
            }

            var qqMenu = contentView.FindViewById(Resource.Id.linear_qq_share);
            if (qqMenu != null)
            {
                qqMenu.Click += async delegate
                 {
                     if (PlatformUtil.IsInstallApp(context, PlatformUtil.QqPackage))
                     {
                         SnackbarUtil.Show(_activity, contentView, "稍后版本将实现该功能");
                         await Task.Delay(2000);
                         Dismiss();
                     }
                     else
                     {
                         SnackbarUtil.Show(_activity, contentView, "您需要QQ客户端");
                     }
                 };
            }
        }

        /// <summary>
        ///  创建文本消息对象
        /// </summary>
        /// <returns></returns>
        private TextObject GetTextObj(string title, string text)
        {
            var textObject = new TextObject
            {
                Text = text,
                Title = title,
                ActionUrl = Constants.BlogUrl
            };
            return textObject;
        }

        /// <summary>
        /// 创建图片消息对象
        /// </summary>
        /// <returns></returns>
        private ImageObject GetImageObj(Context context)
        {
            var imageObject = new ImageObject();
            var bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Mipmap.ic_launcher_round);
            imageObject.SetImageObject(bitmap);
            return imageObject;
        }
    }
}
