using Messages.Replies.CommandResponses;

namespace Irc.Client.Wpf.MessageHandlers.Replies.CommandResponses
{
    public class EndOfNamesReplyMessageHandler : IMessageHandler<EndOfNamesReply>
    {
        public Task HandleAsync(EndOfNamesReply message)
        {
            return Task.CompletedTask;
        }
    }
}
