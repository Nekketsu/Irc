using System;

namespace Irc.Client.Wpf.ViewModels.Tabs.Messages
{
    public class ChatMessageViewModel : MessageViewModel
    {
        private string nickname;

        public ChatMessageViewModel(string nickname, string message) : base(message)
        {
            Nickname = nickname;
        }

        public ChatMessageViewModel(DateTime dateTime, string nickname, string message) : base(dateTime, message)
        {
            Nickname = nickname;
        }

        public string Nickname
        {
            get => nickname;
            set => SetProperty(ref nickname, value);
        }
    }
}
