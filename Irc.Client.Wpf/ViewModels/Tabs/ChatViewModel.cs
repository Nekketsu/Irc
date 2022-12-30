using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public class ChatViewModel : BindableBase
    {
        private string target;
        private ObservableCollection<MessageViewModel> chat;
        private bool isDirty;

        public ChatViewModel(string target)
        {
            Target = target;
            Chat = new();
            IsDirty = false;
        }

        public string Target
        {
            get => target;
            set => SetProperty(ref target, value);
        }

        public ObservableCollection<MessageViewModel> Chat
        {
            get => chat;
            set => SetProperty(ref chat, value);
        }

        public bool IsDirty
        {
            get => isDirty;
            set => SetProperty(ref isDirty, value);
        }
    }
}
