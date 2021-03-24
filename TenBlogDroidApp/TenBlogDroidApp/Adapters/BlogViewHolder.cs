using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DE.Hdodenhof.CircleImageViewLib;

namespace TenBlogDroidApp.Adapters
{
    public class BlogViewHolder : RecyclerView.ViewHolder
    {
        public CircleImageView IvBlogPicture { get; }
        public TextView TvBlogTitle { get; }
        public TextView TvBlogAbstract { get; }
        public TextView TvBlogPublishCategory { get; }

        public BlogViewHolder(View itemView) : base(itemView)
        {
            IvBlogPicture = itemView.FindViewById<CircleImageView>(Resource.Id.iv_blog_picture);
            TvBlogTitle = itemView.FindViewById<TextView>(Resource.Id.tv_blog_title);
            TvBlogAbstract = itemView.FindViewById<TextView>(Resource.Id.tv_blog_abstract);
            TvBlogPublishCategory = itemView.FindViewById<TextView>(Resource.Id.tv_blog_publish_category);
        }
    }
}