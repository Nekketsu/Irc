using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using System.Windows;
using System.Windows.Controls;

namespace Irc.Client.Wpf.DataTemplateSelectors
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ChatMessageDataTemplate { get; set; }
        public DataTemplate MessageDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) => item switch
        {
            ChatMessageViewModel => ChatMessageDataTemplate,
            MessageViewModel => MessageDataTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}
