﻿using System;
using Android.App;
using Android.Content;
using Android.OS;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace TenBlogDroidApp.Widgets
{
    public class SimpleDialogFragment : DialogFragment
    {
        private readonly Context _context;
        private readonly string _message;
        private readonly bool _isShowPositiveBtn;
        private readonly bool _isShowNegativeBtn;
        private readonly string _positiveText;
        private readonly string _negativeText;
        private readonly string _title;
        /// <summary>
        /// 确认按钮点击委托事件
        /// </summary>
        public event EventHandler<DialogClickEventArgs> PositiveClick = null!;
        /// <summary>
        /// 取消按钮点击委托事件
        /// </summary>
        public event EventHandler<DialogClickEventArgs> NegativeClick = null!;

        public SimpleDialogFragment(Context context, string title, string message, string positiveText = "OK", string negativeText = "Cancel", bool isShowPositiveBtn = false, bool isShowNegativeBtn = false)
        {
            _context = context;
            _message = message;
            _title = title;
            _isShowPositiveBtn = isShowPositiveBtn;
            _isShowNegativeBtn = isShowNegativeBtn;
            _positiveText = positiveText;
            _negativeText = negativeText;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(_context);
            builder.SetMessage(_message);
            if (!string.IsNullOrWhiteSpace(_title))
            {
                builder.SetTitle(_title);
            }
            if (_isShowPositiveBtn)
            {
                builder.SetPositiveButton(_positiveText, (EventHandler<DialogClickEventArgs>)null);
            }
            if (_isShowNegativeBtn)
            {
                builder.SetNegativeButton(_negativeText, (EventHandler<DialogClickEventArgs>)null);
            }

            var show = builder.Show();

            if (_isShowPositiveBtn)
            {
                var positiveBtn = show.GetButton((int)DialogButtonType.Positive);
                positiveBtn.Click += PositiveBtn_Click; ;
            }

            if (_isShowNegativeBtn)
            {
                var negativeBtn = show.GetButton((int)DialogButtonType.Negative);
                negativeBtn.Click += NegativeBtn_Click; ;
            }
            return show;
        }

        private void NegativeBtn_Click(object sender, EventArgs e)
        {
            var args = new DialogClickEventArgs(-1);
            NegativeOnClick(args);
            Dismiss();
        }

        private void PositiveBtn_Click(object sender, EventArgs e)
        {
            var args = new DialogClickEventArgs(1);
            PositiveOnClick(args);
            Dismiss();
        }

        /// <summary>
        /// 确认按钮点击委托
        /// </summary>
        /// <param name="args"></param>
        protected void PositiveOnClick(DialogClickEventArgs args) => PositiveClick?.Invoke(this, args);
        /// <summary>
        /// 取消按钮点击委托
        /// </summary>
        /// <param name="args"></param>
        protected void NegativeOnClick(DialogClickEventArgs args) => NegativeClick?.Invoke(this, args);
    }
}