using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Com.Devs.ReadMoreOptionLib;
using Ten.Droid.Library.RecyclerView.Adapters;
using TenBlogDroidApp.RssSubscriber.ImageGetter;
using TenBlogDroidApp.RssSubscriber.Models;
using TenBlogDroidApp.Utils;

namespace TenBlogDroidApp.Adapters
{
    public class BlogRecyclerViewAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        private readonly int _itemViewId;
        private List<Entry> _entries;

        public BlogRecyclerViewAdapter(Context context, List<Entry> entries, int itemViewId)
        {
            _context = context;
            _entries = entries;
            _itemViewId = itemViewId;
        }

        public override int ItemCount => _entries.Count;

        /// <summary>
        ///     Item View点击委托事件
        /// </summary>
        public event EventHandler<RecyclerItemClickEventArgs> ItemClick = null!;

        /// <summary>
        ///     Item View长按委托事件
        /// </summary>
        public event EventHandler<RecyclerItemClickEventArgs> ItemLongClick = null!;

        /// <summary>
        ///     Item view点击事件
        /// </summary>
        /// <param name="args"></param>
        protected void OnClick(RecyclerItemClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        /// <summary>
        ///     Item view长按点击事件
        /// </summary>
        /// <param name="args"></param>
        protected void OnLongClick(RecyclerItemClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is not BlogViewHolder blogViewHolder) return;
            var item = _entries[position];
            var categories = string.Join(", ", from category in item.Categories select category.Term);

            blogViewHolder.IvBlogPicture.SetImageResource(categories.Contains("杂谈")
                ? Resource.Drawable.ic_event_note_black_48dp
                : Resource.Drawable.ic_code_black_48dp);

            if (_context.Resources != null)
            {
                var readMoreOption = new ReadMoreOption.Builder(_context)
                    .TextLength(2, ReadMoreOption.TypeLine)
                    .MoreLabel("展开")
                    .LessLabel("收起")
                    .MoreLabelColor(_context.Resources.GetColor(Resource.Color.colorAccent, null))
                    .LessLabelColor(_context.Resources.GetColor(Resource.Color.colorAccent, null))
                    .LabelUnderLine(true)
                    .ExpandAnimation(true)
                    .Build();
                readMoreOption.AddReadMoreTo(blogViewHolder.TvBlogAbstract,
                    Html.FromHtml(item.Summary.Content, FromHtmlOptions.ModeCompact,
                        new HtmlImageGetter(_context, this), null));
                blogViewHolder.TvBlogPublishCategory.Text =
                    $"{_context.Resources.GetString(Resource.String.fa_calendar_o)} {item.Published:yyyy-MM-dd}   |   {_context.Resources.GetString(Resource.String.fa_folder_o)} {categories}";
            }

            blogViewHolder.TvBlogTitle.Text = item.Title;

            FontManager.MarkAsIconContainer(holder.ItemView, FontManager.GetTypeface(_context, FontManager.FontAwesome),
                TypefaceStyle.Normal);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(_context)?.Inflate(_itemViewId, parent, false);
            return new BlogViewHolder(view, OnClick, OnLongClick);
        }

        /// <summary>
        ///     刷新Adapter里的绑定数据集
        /// </summary>
        /// <param name="items">新的数据集</param>
        public void RefreshItems(List<Entry> items)
        {
            _entries = items;
            NotifyDataSetChanged();
        }
    }
}