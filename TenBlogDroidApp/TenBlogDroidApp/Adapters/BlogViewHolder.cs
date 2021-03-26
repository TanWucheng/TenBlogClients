using System;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using DE.Hdodenhof.CircleImageViewLib;
using Ten.Droid.Library.RecyclerView.Adapters;

namespace TenBlogDroidApp.Adapters
{
    public class BlogViewHolder : RecyclerView.ViewHolder
    {
        public BlogViewHolder(View itemView, Action<RecyclerItemClickEventArgs> onClickAction,
            Action<RecyclerItemClickEventArgs> onLongClickAction) : base(itemView)
        {
            IvBlogPicture = itemView.FindViewById<CircleImageView>(Resource.Id.iv_blog_picture);
            TvBlogTitle = itemView.FindViewById<TextView>(Resource.Id.tv_blog_title);
            TvBlogAbstract = itemView.FindViewById<TextView>(Resource.Id.tv_blog_abstract);
            TvBlogPublishCategory = itemView.FindViewById<TextView>(Resource.Id.tv_blog_publish_category);
            itemView.Click += (_, _) => onClickAction(new RecyclerItemClickEventArgs
            {
                View = itemView,
                Position = AdapterPosition
            });
            itemView.LongClick += (_, _) => onLongClickAction(new RecyclerItemClickEventArgs
            {
                View = itemView,
                Position = AdapterPosition
            });
        }

        public CircleImageView IvBlogPicture { get; }
        public TextView TvBlogTitle { get; }
        public TextView TvBlogAbstract { get; }
        public TextView TvBlogPublishCategory { get; }
    }
}