using System.Linq;
using System.Threading.Tasks;
using Messages.Replies.ErrorReplies;

namespace Irc.Messages.Messages
{
    public class KickMessage : Message
    {
        public string From { get; set; }
        public string ChannelName { get; set; }
        public string Nickname { get; set; }
        public string Message { get; set; }

        public KickMessage(string channelName, string nickname, string message)
        {
            ChannelName = channelName;
            Nickname = nickname;
            Message = message;
        }
        public KickMessage(string from, string channelName, string nickname, string message) : this(channelName, nickname, message)
        {
            From = from;
        }

        public override string ToString()
        {
            return From == null
                ? $"{Command} {ChannelName} :{Nickname}"
                : $":{From} {Command} {ChannelName} :{Nickname}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            var kickChannel = IrcClient.IrcServer.Channels.Values.SingleOrDefault(channel => channel.Name == ChannelName);
            if (kickChannel == null)
            {
                await ircClient.WriteMessageAsync(new NoSuchChannelError(ircClient.Profile.Nickname, ChannelName, NoSuchChannelError.DefaultMessage));
                return true;
            }
            if (!kickChannel.IrcClients.Any(client => client.Profile.Nickname == Nickname))
            {
                await ircClient.WriteMessageAsync(new UserNotInChannelError(ircClient.Profile.Nickname, Nickname, ChannelName, UserNotInChannelError.DefaultMessage));
                return true;
            }
            if (!ircClient.Channels.Values.Any(channel => channel.Name == ChannelName))
            {
                await ircClient.WriteMessageAsync(new NotOnChannelError(ircClient.Profile.Nickname, ChannelName, NotOnChannelError.DefaultMessage));
                return true;
            }

            var kickMessage = new KickMessage(ircClient.Profile.Nickname, ChannelName, Nickname, Message);
            foreach (var client in kickChannel.IrcClients)
            {
                await client.WriteMessageAsync(kickMessage);
            }
            
            return true;
        }

        public new static KickMessage Parse(string message)
        {
            var messageSplit = message.Split();
            var channelName = messageSplit[1];
            var nickname = messageSplit[2];

            var text = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[1]) + messageSplit[1].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[2]) + messageSplit[2].Length).TrimStart();

            if (text.Length == 0)
            {
                text = null;
            }

            return new KickMessage(channelName, nickname, text);
        }
    }
}