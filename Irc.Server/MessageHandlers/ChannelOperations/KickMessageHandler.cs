using Irc.Messages.Messages;
using Messages.Replies.ErrorReplies;

namespace Irc.Server.MessageHandlers.ChannelOperations
{
    public class KickMessageHandler : MessageHandler<KickMessage>
    {
        public async override Task<bool> HandleAsync(KickMessage message, IrcClient ircClient)
        {
            if (ircClient.IrcServer.Channels.TryGetValue(message.ChannelName, out var kickChannel))
            {
                if (kickChannel.IrcClients.TryGetValue(message.Nickname, out var kickClient))
                {
                    if (!ircClient.Channels.ContainsKey(message.ChannelName))
                    {
                        await ircClient.WriteMessageAsync(new NotOnChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, NotOnChannelError.DefaultMessage));
                        return true;
                    }

                    var kickMessage = new KickMessage(ircClient.Profile.Nickname, message.ChannelName, message.Nickname, message.Message);
                    foreach (var client in kickChannel.IrcClients.Values)
                    {
                        await client.WriteMessageAsync(kickMessage);
                    }

                    kickChannel.IrcClients.Remove(message.Nickname);
                    kickClient.Channels.Remove(message.ChannelName);

                    if (!kickChannel.IrcClients.Any())
                    {
                        ircClient.Channels.Remove(message.ChannelName);
                    }
                }
                else
                {
                    await ircClient.WriteMessageAsync(new UserNotInChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.Nickname, message.ChannelName, UserNotInChannelError.DefaultMessage));
                    return true;
                }
            }
            else
            {
                await ircClient.WriteMessageAsync(new NoSuchChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, NoSuchChannelError.DefaultMessage));
                return true;
            }

            return true;
        }
    }
}
