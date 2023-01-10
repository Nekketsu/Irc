using Messages.Replies.CommandResponses;

namespace Irc.Client.MessageHandlers.Received.Replies.CommandResponses
{
    internal class TopicReplyMessageHandler : IMessageHandler<TopicReply>
    {
        public void Handle(TopicReply message, IrcClient ircClient)
        {
            ircClient.Channels[message.ChannelName].Topic = message.Topic;
        }
    }
}
