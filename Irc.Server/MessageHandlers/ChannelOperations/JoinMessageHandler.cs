using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Server.MessageHandlers.ChannelOperations
{
    public class JoinMessageHandler : MessageHandler<JoinMessage>
    {
        public async override Task<bool> HandleAsync(JoinMessage message, IrcClient ircClient)
        {
            Channel channel;

            // Create channel if doesn't exist
            if (!ircClient.IrcServer.Channels.TryGetValue(message.ChannelName, out channel))
            {
                channel = new Channel(message.ChannelName);
                ircClient.IrcServer.Channels.Add(message.ChannelName, channel);
            }

            // Add client to channel and viceversa
            if (!channel.IrcClients.ContainsKey(ircClient.Profile.Nickname))
            {
                channel.IrcClients.Add(ircClient.Profile.Nickname, ircClient);
                ircClient.Channels.Add(message.ChannelName, channel);
            }

            var from = ircClient.Profile.Nickname;
            var joinMessage = new JoinMessage(from, message.ChannelName);
            foreach (var client in channel.IrcClients.Values)
            {
                await client.WriteMessageAsync(joinMessage);
            }

            if (channel.Topic != null)
            {
                await ircClient.WriteMessageAsync(new TopicReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, channel.Name, channel.Topic.TopicMessage));
                await ircClient.WriteMessageAsync(new TopicWhoTimeReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, channel.Name, channel.Topic.Nickname, channel.Topic.SetAt));
            }

            var nicknames = channel.IrcClients.Values.Select(client => client.Profile.Nickname).ToArray();
            await ircClient.WriteMessageAsync(new NameReply(ircClient.IrcServer.ServerName, "=", ircClient.Profile.Nickname, channel.Name, nicknames));
            await ircClient.WriteMessageAsync(new EndOfNamesReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, channel.Name, EndOfNamesReply.DefaultMessage));
            await ircClient.WriteMessageAsync(new CreationTimeReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, channel.Name, channel.CreationTime));

            return true;
        }
    }
}
