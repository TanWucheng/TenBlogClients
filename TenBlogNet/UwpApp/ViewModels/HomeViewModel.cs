using System.Collections.ObjectModel;

namespace UwpApp.ViewModels
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Items = new ObservableCollection<EntryViewModel>();
        }

        public ObservableCollection<EntryViewModel> Items { get; set; }
    }
}