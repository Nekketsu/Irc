using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.Messages;
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
            var queryRequestMessage = new QueryRequestMessage(nickname);
            messenger.Send(queryRequestMessage);
        }

        [RelayCommand]
        private void Whois(string nickname)
        {
            var whoisRequestMessage = new WhoisRequestMessage(nickname);
            messenger.Send(whoisRequestMessage);
        }
    }
}
