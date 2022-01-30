using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using Messages.Replies.ErrorReplies;
using System.Threading.Tasks;

namespace Irc.Server.MessageHandlers.ChannelOperations
{
    public class TopicMessageHandler : MessageHandler<TopicMessage>
    {
        public async override Task<bool> HandleAsync(TopicMessage message, IrcClient ircClient)
        {
            if (ircClient.Channels.TryGetValue(message.ChannelName, out var channel))
            {
                // Query topic
                if (message.Topic == null)
                {
                    if (channel.Topic == null)
                    {
                        await ircClient.WriteMessageAsync(new NoTopicReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, NoTopicReply.DefaultMessage));
                    }
                    else
                    {
                        await ircClient.WriteMessageAsync(new TopicReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, channel.Topic.TopicMessage));
                        await ircClient.WriteMessageAsync(new TopicWhoTimeReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, channel.Topic.Nickname, channel.Topic.SetAt));
                    }
                }
                // Set topic
                else
                {
                    var topic = (message.Topic == string.Empty) ? null : message.Topic;
                    channel.Topic = new Topic(topic, ircClient.Profile.Nickname);

                    var topicMessage = new TopicMessage(ircClient.Profile.Nickname, message.ChannelName, message.Topic);
                    foreach (var client in channel.IrcClients.Values)
                    {
                        await client.WriteMessageAsync(topicMessage);
                    }
                }
            }
            // No on channel
            else if (ircClient.IrcServer.Channels.ContainsKey(message.ChannelName))
            {
                await ircClient.WriteMessageAsync(new NotOnChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, NotOnChannelError.DefaultMessage));
            }
            // No such channel
            else
            {
                await ircClient.WriteMessageAsync(new NoSuchChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, NoSuchChannelError.DefaultMessage));
            }

            return true;
        }
    }
}
