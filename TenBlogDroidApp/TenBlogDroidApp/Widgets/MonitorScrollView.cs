using System;
using Android.Content;
using Android.Util;
using AndroidX.Core.Widget;
using TenBlogDroidApp.Listeners;

namespace TenBlogDroidApp.Widgets
{
    public class MonitorScrollView : NestedScrollView
    {
        private const int Threshold = 20;
        private IFabDisplayListener _displayListener;
        private int _distance;
        private bool _visible;

        public MonitorScrollView(Context context) : base(context)
        {
            Init();
        }

        public MonitorScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public MonitorScrollView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
            defStyleAttr)
        {
            Init();
        }

        public event EventHandler<float> TranslucentChange = null!;

        protected void OnTranslucentChange(float alpha)
        {
            TranslucentChange(this, alpha);
        }

        private void Init()
        {
            _distance = 0;
            _visible = true;
        }

        public void AddFabDisplayListener(IFabDisplayListener displayListener)
        {
            _displayListener = displayListener;
        }

        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {
            base.OnScrollChanged(l, t, oldl, oldt);

            var scrollY = ScrollY;
            if (TranslucentChange != null)
            {
                if (Context?.Resources?.DisplayMetrics == null) return;
                var heightPixels = Context.Resources.DisplayMetrics.HeightPixels;
                if (scrollY <= heightPixels / 3f)
                    //0~1f,而透明度应该是1~0f
                    //alpha=滑出去的高度/(screen_height/3f)
                    OnTranslucentChange(1 - scrollY / (heightPixels / 3f));
            }

            if (_displayListener == null) return;
            var dy = t - oldt;
            switch (_distance)
            {
                case > Threshold when _visible:
                    _visible = false;
                    _displayListener.FabHide();
                    _distance = 0;
                    break;
                case < -20 when !_visible:
                    _visible = true;
                    _displayListener.FabShow();
                    _distance = 0;
                    break;
            }

            if (_visible && dy > 0 || !_visible && dy < 0) _distance += dy;
        }
    }
}