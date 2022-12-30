using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public class StatusViewModel : BindableBase
    {
        private ObservableCollection<string> log;

        public StatusViewModel()
        {
            Log = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Log
        {
            get => log;
            set => SetProperty(ref log, value);
        }
    }
}
