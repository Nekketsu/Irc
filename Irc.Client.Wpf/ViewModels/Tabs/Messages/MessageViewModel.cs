using Prism.Mvvm;
using System;

namespace Irc.Client.Wpf.ViewModels.Tabs.Messages
{
    public class MessageViewModel : BindableBase
    {
        private DateTime dateTime;
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

        public DateTime DateTime
        {
            get => dateTime;
            set => SetProperty(ref dateTime, value);
        }

        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }
    }
}
