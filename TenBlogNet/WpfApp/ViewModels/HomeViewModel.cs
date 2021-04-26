using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Web.WebView2.Wpf;
using TenBlogNet.WpfApp.Domain;

namespace TenBlogNet.WpfApp.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private ICommand _viewBlogArticleCommand;
        private string _indexHtmlUrl;

        public HomeViewModel()
        {
            Items = new ObservableCollection<EntryViewModel>();
            IndexHtmlUrl = $"{ AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/Resources/index.html";
        }

        public string IndexHtmlUrl
        {
            get => _indexHtmlUrl;
            set
            {
                if (_indexHtmlUrl == value) return;
                _indexHtmlUrl = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<EntryViewModel> Items { get; set; }

        public ICommand ViewBlogArticleCommand =>
            _viewBlogArticleCommand ??= new DelegateCommand<Dictionary<string, WebView2>>(ViewBlogArticle);

        public event PropertyChangedEventHandler PropertyChanged;

        private static void ViewBlogArticle(Dictionary<string, WebView2> dictionary)
        {
            if (!dictionary.Any()) return;
            var (url, webView) = dictionary.FirstOrDefault();
            webView.Source = new Uri(url);
        }

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}