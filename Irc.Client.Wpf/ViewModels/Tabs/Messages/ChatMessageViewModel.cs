using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Irc.Client.Wpf.ViewModels.Tabs.Messages
{
    public partial class ChatMessageViewModel : MessageViewModel
    {
        [ObservableProperty]
        private string nickname;

        public ChatMessageViewModel(string nickname, string message) : base(message)
        {
            Nickname = nickname;
        }

        public ChatMessageViewModel(DateTime dateTime, string nickname, string message) : base(dateTime, message)
        {
            Nickname = nickname;
        }
    }
}
