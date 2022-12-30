using CommunityToolkit.Mvvm.ComponentModel;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public partial class StatusViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<MessageViewModel> log;

        public StatusViewModel()
        {
            Log = new ObservableCollection<MessageViewModel>();
        }
    }
}
