using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Google.Android.Material.TextField;

namespace TenBlogNet.AndroidApp.Widgets
{
    public class RemovableAutoCompleteTextView : MaterialAutoCompleteTextView
    {
        private readonly Context _context;
        private Drawable _clearDrawable;
        private Drawable _searchDrawable;

        public RemovableAutoCompleteTextView(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {
            _context = context;
            Init();
        }

        private void Init()
        {
            if (_context.Resources != null)
            {
                _searchDrawable = _context.Resources.GetDrawable(Resource.Drawable.ic_search_black_24dp, null);
                _clearDrawable = _context.Resources.GetDrawable(Resource.Drawable.ic_close_black_24dp, null);
            }

            AfterTextChanged += (_, _) =>
            {
                SetDrawable();
            };
        }

        private void SetDrawable()
        {
            SetCompoundDrawablesWithIntrinsicBounds(_searchDrawable, null, Length() < 1 ? null : _clearDrawable, null);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (_clearDrawable == null || e.Action != MotionEventActions.Up) return base.OnTouchEvent(e);
            var eventX = (int)e.RawX;
            var eventY = (int)e.RawY;
            var rect = new Rect();
            GetGlobalVisibleRect(rect);
            rect.Left = rect.Right - 100;
            if (rect.Contains(eventX, eventY))
            {
                Text = string.Empty;
            }
            return base.OnTouchEvent(e);
        }
    }
}