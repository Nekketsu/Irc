using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Irc.Client.Wpf.ViewModels.Tabs.Messages
{
    public partial class MessageViewModel : ObservableObject
    {
        [ObservableProperty]
        private DateTime dateTime;
        [ObservableProperty]
        private string message;

        public MessageViewModel(string message) : this(DateTime.Now, message)
        {
            Message = message;
        }

        public MessageViewModel(DateTime dateTime, string message)
        {
            DateTime = dateTime;
            Message = message;
        }
    }
}
