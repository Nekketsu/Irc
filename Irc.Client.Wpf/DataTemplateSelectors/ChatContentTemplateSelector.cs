using Irc.Client.Wpf.ViewModels.Tabs;
using System.Windows;
using System.Windows.Controls;

namespace Irc.Client.Wpf.DataTemplateSelectors
{
    public class ChatContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StatusDataTemplate { get; set; }
        public DataTemplate ChannelDataTemplate { get; set; }
        public DataTemplate ChatDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) => item switch
        {
            StatusViewModel => StatusDataTemplate,
            ChannelViewModel => ChannelDataTemplate,
            ChatViewModel => ChatDataTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}
