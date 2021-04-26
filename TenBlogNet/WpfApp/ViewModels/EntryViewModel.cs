using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TenBlogCoreLib.RssSubscriber.Models;

namespace TenBlogNet.WpfApp.ViewModels
{
    public class EntryViewModel : INotifyPropertyChanged
    {
        private string _code;
        private Entry _entry;
        private bool _isSelected;
        private List<CategoryViewModel> _categories;

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// RSS入口(文章)模型
        /// </summary>
        public Entry Entry
        {
            get => _entry;
            set
            {
                if (_entry == value) return;
                _entry = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Blog清单ToggleButton内容
        /// </summary>
        public string Code
        {
            get => _code;
            set
            {
                if (_code == value) return;
                _code = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 文章分类视图模型
        /// </summary>
        public List<CategoryViewModel> Categories
        {
            get => _categories;
            set
            {
                if (_categories == value) return;
                _categories = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}