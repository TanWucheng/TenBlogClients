using System.ComponentModel;
using System.Runtime.CompilerServices;
using FontAwesome.UWP;
using UwpApp.RssSubscriber.Models;

namespace UwpApp.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        private Category _category;
        private FontAwesomeIcon _fontAwesomeIcon;

        public Category Category
        {
            get => _category;
            set
            {
                if (_category == value) return;
                _category = value;
                OnPropertyChanged();
            }
        }

        public FontAwesomeIcon FontAwesomeIcon
        {
            get => _fontAwesomeIcon;
            set
            {
                if (_fontAwesomeIcon == value) return;
                _fontAwesomeIcon = value;
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