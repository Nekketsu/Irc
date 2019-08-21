using System.Linq;
using System.Threading.Tasks;
using Irc.Extensions;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class JoinMessage : Message
    {
        public string ChannelName { get; set; }
        public string From { get; set; }

        public JoinMessage(string channelName)
        {
            ChannelName = channelName;
        }

        public JoinMessage(string from, string channelName) : this(channelName)
        {
            From = from;
        }

        public override string ToString()
        {
            return (From == null)
                ? $"{Command} {ChannelName}"
                : $":{From} {Command} {ChannelName}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            Channel channel;
            
            // Create channel if doesn't exist
            if (!IrcClient.IrcServer.Channels.TryGetValue(ChannelName, out channel))
            {
                channel = new Channel(ChannelName);
                IrcClient.IrcServer.Channels.Add(ChannelName, channel);
            }

            // Add client to channel and viceversa
            if (!channel.IrcClients.Contains(ircClient))
            {
                channel.IrcClients.Add(ircClient);
                ircClient.Channels.Add(ChannelName, channel);
            }

            var from = ircClient.Profile.Nickname;
            var joinMessage = new JoinMessage(from, ChannelName);
            foreach (var client in channel.IrcClients)
            {
                await client.WriteMessageAsync(joinMessage);
            }

            if (channel.Topic != null)
            {
                await ircClient.WriteMessageAsync(new TopicReply(ircClient.Profile.Nickname, channel.Name, channel.Topic.TopicMessage));
                await ircClient.WriteMessageAsync(new TopicWhoTimeReply(ircClient.Profile.Nickname, channel.Name, channel.Topic.Nickname, channel.Topic.SetAt));
            }

            var nicknames = channel.IrcClients.Select(client => client.Profile.Nickname).ToArray();
            await ircClient.WriteMessageAsync(new NameReply(ircClient.Profile.Nickname, channel.Name, nicknames));
            await ircClient.WriteMessageAsync(new EndOfNamesReply(ircClient.Profile.Nickname, channel.Name, "End of NAMES list"));
            await ircClient.WriteMessageAsync(new CreationTimeReply(ircClient.Profile.Nickname, channel.Name, channel.CreationTime));

            return true;
        }
    }
}