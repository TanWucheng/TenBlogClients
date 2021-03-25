using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace TenBlogNet.AndroidApp.Utils
{
    internal class FontManager
    {
        private const string Root = "fonts/";
        public const string FontAwesome = Root + "fontawesome-webfont.ttf";

        public static Typeface GetTypeface(Context context, string font)
        {
            return Typeface.CreateFromAsset(context.Assets, font);
        }

        public static void MarkAsIconContainer(View v, Typeface typeface, TypefaceStyle typefaceStyle)
        {
            switch (v)
            {
                case ViewGroup viewGroup:
                    {
                        for (var i = 0; i < viewGroup.ChildCount; i++)
                        {
                            var child = viewGroup.GetChildAt(i);
                            MarkAsIconContainer(child, typeface, typefaceStyle);
                        }

                        break;
                    }
                case TextView textView:
                    textView.SetTypeface(typeface, typefaceStyle);
                    break;
            }
        }
    }
}
