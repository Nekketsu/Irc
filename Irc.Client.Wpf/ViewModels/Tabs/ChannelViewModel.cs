using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public class ChannelViewModel : ChatViewModel
    {
        private ObservableCollection<string> users;

        public ChannelViewModel(string target) : base(target)
        {
        }

        public ObservableCollection<string> Users
        {
            get => users;
            set => SetProperty(ref users, value);
        }
    }
}
