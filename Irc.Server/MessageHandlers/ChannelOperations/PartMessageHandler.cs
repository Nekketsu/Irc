using Irc.Messages.Messages;
using Messages.Replies.ErrorReplies;
using System.Linq;
using System.Threading.Tasks;

namespace Irc.Server.MessageHandlers.ChannelOperations
{
    public class PartMessageHandler : MessageHandler<PartMessage>
    {
        public async override Task<bool> HandleAsync(PartMessage message, IrcClient ircClient)
        {
            if (ircClient.Channels.TryGetValue(message.ChannelName, out var partChannel))
            {
                var partMessage = new PartMessage(ircClient.Profile.Nickname, message.ChannelName, message.Message);
                foreach (var client in partChannel.IrcClients.Values)
                {
                    await client.WriteMessageAsync(partMessage);
                }

                partChannel.IrcClients.Remove(ircClient.Profile.Nickname);
                ircClient.Channels.Remove(message.ChannelName);
                if (!partChannel.IrcClients.Any())
                {
                    ircClient.IrcServer.Channels.Remove(message.ChannelName);
                }
            }
            else if (ircClient.IrcServer.Channels.ContainsKey(message.ChannelName))
            {
                await ircClient.WriteMessageAsync(new NotOnChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, NotOnChannelError.DefaultMessage));
            }
            else
            {
                await ircClient.WriteMessageAsync(new NoSuchChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, NoSuchChannelError.DefaultMessage));
            }

            return true;
        }
    }
}
