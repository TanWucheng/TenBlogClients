using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Google.Android.Material.AppBar;
using Java.Lang;
using Java.Lang.Reflect;
using Object = Java.Lang.Object;

namespace TenBlogDroidApp.Utils
{
    public class AppBarLayoutBehavior : AppBarLayout.Behavior
    {
        private const int TypeFling = 1;
        private bool _isFlinging;
        private bool _shouldBlockNestedScroll;

        public AppBarLayoutBehavior(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        //public override bool OnInterceptTouchEvent(CoordinatorLayout parent, Object child, MotionEvent ev)
        //{
        //    _shouldBlockNestedScroll = false;
        //    if (_isFlinging)
        //    {
        //        _shouldBlockNestedScroll = true;
        //    }

        //    switch (ev.ActionMasked)
        //    {
        //        case MotionEventActions.Down:
        //            {
        //                if (child is AppBarLayout appBarLayout)
        //                {
        //                    StopAppBarLayoutFling(appBarLayout);//手指触摸屏幕的时候停止fling事件
        //                }
        //                break;
        //            }
        //    }

        //    return base.OnInterceptTouchEvent(parent, child, ev);
        //}

        /// <summary>
        /// 反射获取私有的flingRunnable 属性，考虑support 28以后变量名修改的问题
        /// </summary>
        /// <returns></returns>
        private Field GetFlingRunnableField()
        {
            try
            {
                // support design 27及一下版本
                var headerBehaviorType = Class.Superclass.Superclass;
                return headerBehaviorType.GetDeclaredField("mFlingRunnable");
            }
            catch (NoSuchFieldException)
            {
                // 可能是28及以上版本
                var headerBehaviorType = Class.Superclass.Superclass.Superclass;
                return headerBehaviorType.GetDeclaredField("flingRunnable");
            }
        }

        /// <summary>
        /// 反射获取私有的scroller 属性，考虑support 28以后变量名修改的问题
        /// </summary>
        /// <returns></returns>
        private Field GetScrollerField()
        {
            try
            {
                // support design 27及一下版本
                var headerBehaviorType = Class.Superclass.Superclass;
                return headerBehaviorType.GetDeclaredField("mScroller");
            }
            catch (NoSuchFieldException)
            {
                // 可能是28及以上版本
                var headerBehaviorType = Class.Superclass.Superclass.Superclass;
                return headerBehaviorType.GetDeclaredField("scroller");
            }
        }

        /// <summary>
        /// 停止AppBarLayout的fling事件
        /// </summary>
        /// <param name="appBarLayout"></param>
        private void StopAppBarLayoutFling(AppBarLayout appBarLayout)
        {
            //通过反射拿到HeaderBehavior中的flingRunnable变量
            try
            {
                Field flingRunnableField = GetFlingRunnableField();
                Field scrollerField = GetScrollerField();
                flingRunnableField.Accessible = true;
                scrollerField.Accessible = true;

                Runnable flingRunnable = (Runnable)flingRunnableField.Get(this);
                OverScroller overScroller = (OverScroller)scrollerField.Get(this);
                if (flingRunnable != null)
                {
                    appBarLayout.RemoveCallbacks(flingRunnable);
                    flingRunnableField.Set(this, null);
                }
                if (overScroller != null && !overScroller.IsFinished)
                {
                    overScroller.AbortAnimation();
                }
            }
            catch (NoSuchFieldException)
            {
            }
            catch (IllegalAccessException)
            {
            }
        }

        public override bool OnStartNestedScroll(CoordinatorLayout parent, Object child, View directTargetChild, View target, int nestedScrollAxes, int type)
        {
            if (child is AppBarLayout appBarLayout)
            {
                StopAppBarLayoutFling(appBarLayout);
            }
            return base.OnStartNestedScroll(parent, child, directTargetChild, target, nestedScrollAxes, type);
        }

        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Object child, View target, int dx, int dy, int[] consumed, int type)
        {
            //type返回1时，表示当前target处于非touch的滑动，
            //该bug的引起是因为appbar在滑动时，CoordinatorLayout内的实现NestedScrollingChild2接口的滑动子类还未结束其自身的fling
            //所以这里监听子类的非touch时的滑动，然后block掉滑动事件传递给AppBarLayout
            if (type == TypeFling)
            {
                _isFlinging = true;
            }
            if (!_shouldBlockNestedScroll)
            {
                base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed, type);
            }
        }

        //[Obsolete]
        //public override void OnNestedScroll(CoordinatorLayout coordinatorLayout, Object child, View target, int dx, int dy, int consumed, int type, int p7)
        //{
        //    if (!_shouldBlockNestedScroll)
        //    {
        //        base.OnNestedScroll(coordinatorLayout, child, target, dx, dy, consumed, type, p7);
        //    }
        //}

        public override void OnStopNestedScroll(CoordinatorLayout coordinatorLayout, Object abl, View target, int type)
        {
            base.OnStopNestedScroll(coordinatorLayout, abl, target, type);
            _isFlinging = false;
            _shouldBlockNestedScroll = false;
        }
    }
}
