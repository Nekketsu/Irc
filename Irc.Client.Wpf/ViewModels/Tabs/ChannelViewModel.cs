using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public partial class ChannelViewModel : ChatViewModel
    {
        [ObservableProperty]
        private ObservableCollection<string> users;

        public ChannelViewModel(string target) : base(target)
        {
        }
    }
}
