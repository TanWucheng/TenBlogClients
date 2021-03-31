using System.Collections.Generic;
using Android.App;
using Android.Views;
using AndroidX.Core.Util;

namespace TenBlogDroidApp.Utils
{
    public class TransitionUtil
    {
        public static Pair[] CreateSafeTransitionParticipants(Activity activity, bool includeStatusBar, params Pair[] otherParticipants)
        {
            // Avoid system UI glitches as described here:
            // https://plus.google.com/+AlexLockwood/posts/RPtwZ5nNebb
            if (activity.Window == null) return new Pair[] { };
            var decor = activity.Window.DecorView;
            View statusBar = null;
            if (includeStatusBar)
            {
                statusBar = decor.FindViewById(Android.Resource.Id.StatusBarBackground);
            }
            var navBar = decor.FindViewById(Android.Resource.Id.NavigationBarBackground);

            // 创建一对过渡参与者。
            List<Pair> participants = new();
            AddNonNullViewToTransitionParticipants(statusBar, participants);
            AddNonNullViewToTransitionParticipants(navBar, participants);
            // only add transition participants if there's at least one none-null element
            // 只有添加过渡参与者，如果至少有一个非空元素
            if (otherParticipants != null && !(otherParticipants.Length == 1 && otherParticipants[0] == null))
            {
                participants.AddRange(otherParticipants);
            }
            return participants.ToArray();

        }

        private static void AddNonNullViewToTransitionParticipants(View view, ICollection<Pair> participants)
        {
            if (view == null)
            {
                return;
            }
            participants.Add(new Pair(view, view.TransitionName));
        }
    }
}
