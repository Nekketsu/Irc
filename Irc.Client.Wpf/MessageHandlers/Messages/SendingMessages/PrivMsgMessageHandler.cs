using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Irc.Messages.Messages;
using System;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Messages
{
    public class PrivMsgMessageHandler : IMessageHandler<PrivMsgMessage>
    {
        private readonly IrcViewModel viewModel;

        public PrivMsgMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(PrivMsgMessage message)
        {
            string from;
            string target;

            if (message.From is null)
            {
                from = viewModel.Nickname;
                target = message.Target;
            }
            else
            {
                from = viewModel.Irc.GetNickName(message.From);
                target = message.Target.Equals(viewModel.Nickname, StringComparison.InvariantCultureIgnoreCase)
                    ? from
                    : message.Target;
            }

            var messageViewModel = new ChatMessageViewModel(from, message.Text);
            viewModel.DrawMessage(target, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
