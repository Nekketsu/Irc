using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public class StatusViewModel : BindableBase
    {
        private ObservableCollection<MessageViewModel> log;

        public StatusViewModel()
        {
            Log = new ObservableCollection<MessageViewModel>();
        }

        public ObservableCollection<MessageViewModel> Log
        {
            get => log;
            set => SetProperty(ref log, value);
        }
    }
}
