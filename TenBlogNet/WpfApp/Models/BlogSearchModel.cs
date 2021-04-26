using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TenBlogNet.WpfApp.Models
{
    public class BlogSearchModel : INotifyPropertyChanged
    {
        private string _link;

        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get => _link;
            set
            {
                if (_link == value) return;
                _link = value;
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