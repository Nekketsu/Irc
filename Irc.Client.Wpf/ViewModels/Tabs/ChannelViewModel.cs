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
        private void QueryNickname(string nickname)
        {
            var queryNicknameMessages = new QueryNicknameMessage(nickname);
            messenger.Send(queryNicknameMessages);
        }
    }
}
