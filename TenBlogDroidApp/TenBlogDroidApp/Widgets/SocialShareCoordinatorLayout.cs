using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Com.Sina.Weibo.Sdk;
using Com.Sina.Weibo.Sdk.Api;
using Com.Sina.Weibo.Sdk.Auth;
using Com.Sina.Weibo.Sdk.Share;
using Ten.Droid.Library.Utils;
using TenBlogDroidApp.Utils;
using Uri = Android.Net.Uri;

namespace TenBlogDroidApp.Widgets
{
    public class SocialShareCoordinatorLayout : CoordinatorLayout, IWbShareCallback
    {
        private const string LogTag = nameof(SocialShareCoordinatorLayout);

        private WbShareHandler _shareHandler;
        private string _blogUrl;

        /// <summary>
        /// 社交分享图标点击委托事件
        /// </summary>
        public event EventHandler<object> ShareMenuClick = null!;

        protected void OnShareMenuClick(object menu)
        {
            ShareMenuClick?.Invoke(this, menu);
        }

        private void InitWeiboSdk()
        {
            // 创建微博API接口类对象
            WbSdk.Install(Context, new AuthInfo(Context, Constants.SinaAppKey, Constants.BlogUrl, string.Empty));
            _shareHandler = new WbShareHandler((Activity)Context);
            _shareHandler.RegisterApp();
        }

        public SocialShareCoordinatorLayout(Context context, string blogUrl) : base(context)
        {
            Init(blogUrl);
        }

        public SocialShareCoordinatorLayout(Context context, IAttributeSet attrs, string blogUrl) : base(context, attrs)
        {
            Init(blogUrl);
        }

        public SocialShareCoordinatorLayout(Context context, IAttributeSet attrs, int defStyleAttr, string blogUrl) : base(context, attrs,
            defStyleAttr)
        {
            Init(blogUrl);
        }

        private void Init(string blogUrl)
        {
            _blogUrl = blogUrl;
            LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            var view = LayoutInflater.From(Context)?.Inflate(Resource.Layout.social_share_menu, null);
            if (view != null)
            {
                InitShareMenu(view);
                AddView(view, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
                InitWeiboSdk();
            }
        }

        public void OnWbShareCancel()
        {
            Log.Debug(LogTag, "微博分享取消");
        }

        public void OnWbShareFail()
        {
            Log.Debug(LogTag, "微博分享取消");
        }

        public void OnWbShareSuccess()
        {
            Log.Debug(LogTag, "微博分享成功");
        }

        public void InitShareMenu(View contentView)
        {
            var sysShareMenu = contentView.FindViewById(Resource.Id.linear_sys_share);
            if (sysShareMenu != null)
            {
                sysShareMenu.Click += delegate
                {
                    var intent = new Intent(Intent.ActionSend);
                    intent.SetType("text/plain");
                    intent.PutExtra(Intent.ExtraTitle, "分享博客网址");
                    intent.PutExtra(Intent.ExtraText, _blogUrl);
                    Context.StartActivity(Intent.CreateChooser(intent, "分享到"));
                    OnShareMenuClick(sysShareMenu);
                };
            }

            var copyUrlMenu = contentView.FindViewById(Resource.Id.linear_copy_url);
            if (copyUrlMenu != null)
            {
                copyUrlMenu.Click += delegate
                {
                    if (Context.GetSystemService(Context.ClipboardService) is not ClipboardManager manager) return;
                    var data = ClipData.NewPlainText("shareUrl", _blogUrl);
                    manager.PrimaryClip = data;
                    ToastUtil.Show(Context, "链接已复制到剪贴板");

                    OnShareMenuClick(copyUrlMenu);
                };
            }

            var smsShareMenu = contentView.FindViewById(Resource.Id.linear_sms_share);
            if (smsShareMenu != null)
            {
                smsShareMenu.Click += delegate
                {
                    var uri = Uri.Parse("smsto:");
                    var intent = new Intent(Intent.ActionSendto, uri);
                    intent.PutExtra("sms_body", $"来自Ten's Blog的分享短信，欢迎访问的我的博客网站：{_blogUrl}");
                    Context.StartActivity(intent);

                    OnShareMenuClick(smsShareMenu);
                };
            }

            var emailShareMenu = contentView.FindViewById(Resource.Id.linear_email_share);
            if (emailShareMenu != null)
            {
                emailShareMenu.Click += delegate
                {
                    var intent = new Intent(Intent.ActionSend);
                    intent.SetData(Uri.Parse(Constants.MailToMe));
                    intent.PutExtra(Intent.ExtraSubject, "欢迎访问我的博客网站");
                    intent.PutExtra(Intent.ExtraText, $"<h5>来自Ten's Blog的分享邮件</h5><p><a href='{_blogUrl}'>点此</a>访问博客网站</p>");
                    Context.StartActivity(Intent.CreateChooser(intent, "选择邮箱客户端"));

                    OnShareMenuClick(emailShareMenu);
                };
            }

            var weChatMenu = contentView.FindViewById(Resource.Id.linear_wechat_share);
            if (weChatMenu != null)
            {
                weChatMenu.Click += delegate
                {
                    if (PlatformUtil.IsInstallApp(Context, PlatformUtil.WeChatPackage))
                    {
                        Intent intent = new();
                        ComponentName cop = new(PlatformUtil.WeChatPackage, "com.tencent.mm.ui.tools.ShareImgUI");
                        intent.SetComponent(cop);
                        intent.SetAction(Intent.ActionSend);
                        intent.PutExtra("android.intent.extra.TEXT", _blogUrl);
                        intent.PutExtra("Kdescription", "Ten's Blog网站地址");
                        intent.SetFlags(ActivityFlags.NewTask);
                        Context.StartActivity(intent);

                        OnShareMenuClick(weChatMenu);
                    }
                    else
                    {
                        SnackbarUtil.Show(Context, contentView, "您需要安装微信客户端");
                    }
                };
            }

            var weiboMenu = contentView.FindViewById(Resource.Id.linear_weibo_share);
            if (weiboMenu != null)
            {
                weiboMenu.Click += delegate
                {
                    if (PlatformUtil.IsInstallApp(Context, PlatformUtil.WeiboPackage))
                    {
                        WeiboMultiMessage weiboMessage = new() { ImageObject = GetImageObj(Context) };
                        if (Context.Resources != null)
                            weiboMessage.TextObject = GetTextObj(Context.Resources.GetString(Resource.String.app_name_cn), "来自Ten's Blog的分享");
                        _shareHandler.ShareMessage(weiboMessage, false);

                        OnShareMenuClick(weiboMenu);
                    }
                    else
                    {
                        SnackbarUtil.Show(Context, contentView, "您需要安装微博客户端");
                    }
                };
            }

            var qqMenu = contentView.FindViewById(Resource.Id.linear_qq_share);
            if (qqMenu != null)
            {
                qqMenu.Click += async delegate
                {
                    if (PlatformUtil.IsInstallApp(Context, PlatformUtil.QqPackage))
                    {
                        SnackbarUtil.Show(Context, contentView, "稍后版本将实现该功能");
                        await Task.Delay(2000);
                        OnShareMenuClick(qqMenu);
                    }
                    else
                    {
                        SnackbarUtil.Show(Context, contentView, "您需要QQ客户端");
                    }
                };
            }
        }

        public void SetBlogUrl(string blogUrl)
        {
            _blogUrl = blogUrl;
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
                ActionUrl = _blogUrl
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
