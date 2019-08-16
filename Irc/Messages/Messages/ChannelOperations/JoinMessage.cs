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

            var from = ircClient.Profile.NickName;
            var joinMessage = new JoinMessage(from, ChannelName);
            foreach (var client in channel.IrcClients)
            {
                await client.WriteMessageAsync(joinMessage);
            }

            if (channel.Topic != null)
            {
                await ircClient.WriteMessageAsync(new TopicReply(ircClient.Profile.NickName, channel.Name, channel.Topic.TopicMessage));
                await ircClient.WriteMessageAsync(new TopicWhoTimeReply(ircClient.Profile.NickName, channel.Name, channel.Topic.NickName, channel.Topic.SetAt));
            }

            var nickNames = channel.IrcClients.Select(client => client.Profile.NickName).ToArray();
            await ircClient.WriteMessageAsync(new NameReply(ircClient.Profile.NickName, channel.Name, nickNames));
            await ircClient.WriteMessageAsync(new EndOfNamesReply(ircClient.Profile.NickName, channel.Name, "End of NAMES list"));
            await ircClient.WriteMessageAsync(new CreationTimeReply(ircClient.Profile.NickName, channel.Name, channel.CreationTime));

            return true;
        }
    }
}