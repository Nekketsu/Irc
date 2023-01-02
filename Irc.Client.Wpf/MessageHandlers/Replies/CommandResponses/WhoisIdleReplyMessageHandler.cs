using Irc.Client.Wpf.ViewModels;
using Irc.Client.Wpf.ViewModels.Tabs.Messages;
using Messages.Replies.CommandResponses;
using System.Globalization;
using System.Threading.Tasks;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class WhoisIdleReplyMessageHandler : IMessageHandler<WhoisIdleReply>
    {
        private readonly IrcViewModel viewModel;

        public WhoisIdleReplyMessageHandler(IrcViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public Task HandleAsync(WhoisIdleReply message)
        {
            var cultureInfo = new CultureInfo("en-US");

            var text = $"{message.Nickname} has been idle";
            if (message.Idle.TotalHours >= 1)
            {
                text += $" {(int)message.Idle.TotalHours}hrs";
            }
            if (message.Idle.TotalMinutes >= 1)
            {
                text += $" {message.Idle.Minutes}mins";
            }
            text += $" {message.Idle.Seconds}secs";

            text += $", sign on {message.SignOn.ToString("ddd MMM dd hh: mm:ss", cultureInfo)}";

            var messageViewModel = new MessageViewModel(text);
            viewModel.DrawMessage(viewModel.Status, messageViewModel);

            return Task.CompletedTask;
        }
    }
}
