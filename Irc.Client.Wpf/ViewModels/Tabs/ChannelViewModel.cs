using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.Messenger.Requests;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public partial class ChannelViewModel : ChatViewModel, INotifyPropertyChanged
    {
        [ObservableProperty]
        private ObservableCollection<string> users;

        public ChannelViewModel(string target) : base(target)
        {
        }

        [RelayCommand]
        private void Query(string nickname)
        {
            var queryRequest = new QueryRequest(nickname);
            messenger.Send(queryRequest);
        }
    }
}
