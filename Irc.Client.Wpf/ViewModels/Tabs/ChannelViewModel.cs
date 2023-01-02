using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.Messenger.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public partial class ChannelViewModel : ChatViewModel
    {
        [ObservableProperty]
        private ObservableCollection<string> users;

        private IMessenger messenger;

        public ChannelViewModel(string target) : base(target)
        {
            messenger = App.Current.Services.GetService<IMessenger>();
        }

        [RelayCommand]
        private void Query(string nickname)
        {
            var queryRequest = new QueryRequest(nickname);
            messenger.Send(queryRequest);
        }

        [RelayCommand]
        private void Whois(string nickname)
        {
            var whoisRequest = new WhoisRequest(nickname);
            messenger.Send(whoisRequest);
        }
    }
}
