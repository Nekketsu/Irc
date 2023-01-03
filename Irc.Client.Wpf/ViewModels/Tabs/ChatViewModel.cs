using CommunityToolkit.Mvvm.ComponentModel;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;

namespace Irc.Client.Wpf.ViewModels.Tabs
{
    public partial class ChatViewModel : ObservableObject, ITabViewModel
    {
        [ObservableProperty]
        private string target;

        [ObservableProperty]
        private ObservableCollection<MessageViewModel> chat;

        [ObservableProperty]
        private bool isDirty;

        public ChatViewModel(string target)
        {
            Target = target;
            Chat = new();
            IsDirty = false;
        }
    }
}
