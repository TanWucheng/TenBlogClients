using System;
using System.IO;
using Android;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Widget;
using AndroidX.Core.Content;
using TenBlogNet.AndroidApp.Adapters;
using Volley.Toolbox;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Permission = Android.Content.PM.Permission;

namespace TenBlogNet.AndroidApp.RssSubscriber.ImageGetter
{
    internal class HtmlImageGetter : Java.Lang.Object, Html.IImageGetter
    {
        private readonly Context _context;
        private readonly BlogRecyclerViewAdapter _adapter;

        public HtmlImageGetter(Context context, BlogRecyclerViewAdapter adapter)
        {
            _context = context;
            _adapter = adapter;
        }

        public Drawable GetDrawable(string source)
        {
            var fileName = GetFileName(source);
            Drawable drawable = null;
            var absFilePath = $"{_context.GetExternalFilesDir(Environment.DirectoryPictures)}";
            var file = new File(absFilePath, fileName);
            if (file.Exists())
            {
                drawable = Drawable.CreateFromPath(file.AbsolutePath);
                drawable?.SetBounds(0, 0, drawable.IntrinsicWidth * 2,
                    drawable.IntrinsicHeight * 2);
            }
            else
            {
                var hasWriteExternalPermission = ContextCompat.CheckSelfPermission(_context, Manifest.Permission.WriteExternalStorage);
                if (hasWriteExternalPermission != Permission.Granted) return null;
                GetNetworkImg(source);
            }
            return drawable;
        }

        private void GetNetworkImg(string url)
        {

            var requestQueue = Volley.Toolbox.Volley.NewRequestQueue(_context);
            var imageRequest = new ImageRequest(url, new VolleyResponseListener(GetFileName(url), this)
                , 0, 0, ImageView.ScaleType.Center, Bitmap.Config.Rgb565, new VolleyResponseErrorListener(_context));
            requestQueue.Add(imageRequest);
        }

        private static string GetFileName(string path)
        {
            var pos1 = path.LastIndexOf('/');
            var pos2 = path.LastIndexOf('\\');
            var pos = Math.Max(pos1, pos2);
            var str = pos < 0 ? path : path[(pos + 1)..];

            return str;
        }

        public void SaveBitmap(string fileName, Bitmap bitmap)
        {
            var sdCardPath = _context.GetExternalFilesDir(Environment.DirectoryPictures)?.AbsolutePath;
            var filePath = System.IO.Path.Combine(sdCardPath ?? string.Empty, fileName);
            var stream = new FileStream(filePath, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();
            _adapter.NotifyDataSetChanged();
        }
    }
}