using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irc.Client.Wpf.Messenger.Requests;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public partial class ChatViewModel : ObservableObject, ITabViewModel
    {
        protected IMessenger messenger;

        [ObservableProperty]
        private string target;

        [ObservableProperty]
        private ObservableCollection<MessageViewModel> chat;

        [ObservableProperty]
        private bool isDirty;

        [RelayCommand]
        private void Whois(string nickname)
        {
            var whoisRequest = new WhoisRequest(nickname);
            messenger.Send(whoisRequest);
        }

        public ChatViewModel(string target)
        {
            Target = target;
            Chat = new();
            IsDirty = false;

            messenger = App.Current.Services.GetService<IMessenger>();
        }
    }
}
