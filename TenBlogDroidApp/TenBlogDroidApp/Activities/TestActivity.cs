using System.Collections.Generic;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using Com.Devs.ReadMoreOptionLib;
using Ten.Droid.Library.Extensions;
using TenBlogDroidApp.Utils;

namespace TenBlogDroidApp.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class TestActivity : Activity
    {
        private ConstraintLayout _rootView;
        private bool _expandState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_test);

            InitView();
        }

        private void InitView()
        {
            _rootView = FindViewById<ConstraintLayout>(Resource.Id.constraintLayout_test);
            FontManager.MarkAsIconContainer(_rootView, FontManager.GetTypeface(this, FontManager.FontAwesome), TypefaceStyle.Normal);

            var items = new List<string>
            {
                "<h1>望岳</h1><p><a>杜甫</a></p><p>岱宗夫如何？齐鲁青未了。</p><p>造化钟神秀，阴阳割昏晓。</p><p>会当凌绝顶，一览众山小。</p><p>荡胸生层云，决眦入归鸟。</p>",
                "<h1>出塞</h1><p>王昌龄</p><p>秦时明月汉时关，万里长征人未还。</p><p>但使龙城飞将在，不教胡马度阴山。</p>",
                "<h1>早发白帝城</h1><p>李白</p><p>朝辞白帝彩云间，千里江陵一日还。</p><p>两岸猿声啼不住，轻舟已过万重山。</p><p><img src=\"http://i1.fuimg.com/734144/4c6a8dbf96ada12d.png\" alt=\"未找到图片\"/></p>",
                "<h1>枫桥夜泊</h1><p>张继</p><p>月落乌啼霜满天，江枫渔火对愁眠。</p><p>姑苏城外寒山寺，夜半钟声到客船。</p><p><img src=\"http://i1.fuimg.com/734144/9dedad1db49ccc09.png\" alt=\"未找到图片\"/></p>",
                "<h1>卜算子・咏梅</h1><p>毛泽东 〔近现代〕</p><p>风雨送春归，飞雪迎春到。</p><p>已是悬崖百丈冰，犹有花枝俏。</p><p>俏也不争春，只把春来报。</p><p>待到山花烂漫时，她在丛中笑。</p>"
            };

            var tvAutoExpand = FindViewById<TextView>(Resource.Id.tv_auto_expand);
            if (Resources != null)
            {
                var readMoreOption = new ReadMoreOption.Builder(this)
                    .TextLength(20, ReadMoreOption.TypeCharacter)
                    .MoreLabel("展开")
                    .LessLabel("收起")
                    .MoreLabelColor(Resources.GetColor(Resource.Color.colorAccent, null))
                    .LessLabelColor(Resources.GetColor(Resource.Color.colorAccent, null))
                    .LabelUnderLine(true)
                    .ExpandAnimation(true)
                    .Build();
                readMoreOption.AddReadMoreTo(tvAutoExpand, Html.FromHtml(items[0], FromHtmlOptions.ModeCompact, null, null));
            }

            var tvExpandSource = FindViewById<TextView>(Resource.Id.tv_expand_source);
            tvExpandSource.SetHtml(items[4]);

            var tvExpandAction = FindViewById<TextView>(Resource.Id.tv_expand_action);
            if (tvExpandAction != null)
                tvExpandAction.Click += delegate
                {
                    // 未展开
                    if (!_expandState)
                    {
                        if (tvExpandSource != null)
                        {
                            tvExpandSource.Ellipsize = null;
                            tvExpandSource.SetSingleLine(false);
                        }

                        tvExpandAction.SetText(Resource.String.fa_chevron_up);
                    }
                    else
                    {
                        if (tvExpandSource != null)
                        {
                            tvExpandSource.Ellipsize = TextUtils.TruncateAt.End;
                            tvExpandSource.SetLines(2);
                        }

                        tvExpandAction.SetText(Resource.String.fa_chevron_down);
                    }

                    _expandState = !_expandState;
                };
        }
    }
}