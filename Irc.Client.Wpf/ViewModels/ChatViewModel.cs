using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace Irc.Client.Wpf.ViewModels
{
    public class ChatViewModel : BindableBase
    {
        private string target;
        private string title;
        private ObservableCollection<string> chat;
        private bool isDirty;

        public ChatViewModel(string title, string target = null)
        {
            Target = target;
            Title = title;
            Chat = new ObservableCollection<string>();
            IsDirty = false;
        }

        public string Target
        {
            get => target;
            set => SetProperty(ref target, value);
        }

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public ObservableCollection<string> Chat
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
