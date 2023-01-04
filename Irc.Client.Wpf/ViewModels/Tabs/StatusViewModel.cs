using CommunityToolkit.Mvvm.ComponentModel;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public partial class StatusViewModel : ObservableObject, ITabViewModel
    {
        [ObservableProperty]
        private ObservableCollection<MessageViewModel> log;

        [ObservableProperty]
        private bool isDirty;

        public StatusViewModel()
        {
            Log = new();
        }
    }
}
