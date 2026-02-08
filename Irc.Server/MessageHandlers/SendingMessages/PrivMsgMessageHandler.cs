using Irc.Messages.Messages;
using Messages.Replies.ErrorReplies;

namespace Irc.Server.MessageHandlers.SendingMessages
{
    public class PrivMsgMessageHandler : MessageHandler<PrivMsgMessage>
    {
        public async override Task<bool> HandleAsync(PrivMsgMessage message, IrcClient ircClient)
        {
            if (message.Target.StartsWith("#"))
            {
                await SendMessageToChannel(message, ircClient);
            }
            else
            {
                await SendMessageToUser(message, ircClient);
            }

            return true;
        }

        private async Task SendMessageToChannel(PrivMsgMessage message, IrcClient ircClient)
        {
            if (ircClient.IrcServer.Channels.TryGetValue(message.Target, out var targetChannel))
            {
                var from = ircClient.Profile.Nickname;
                var privMsgMessage = new PrivMsgMessage(from, message.Target, message.Text);
                foreach (var targetClient in targetChannel.IrcClients.Values.Where(client => client != ircClient))
                {
                    await targetClient.WriteMessageAsync(privMsgMessage);
                }
            }
            else
            {
                await ircClient.WriteMessageAsync(new CannotSendToChannelError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.Target, "Cannot send to channel"));
            }
        }

        private async Task SendMessageToUser(PrivMsgMessage message, IrcClient ircClient)
        {
            var targetClient = ircClient.IrcServer.IrcClients.SingleOrDefault(client => client.Profile.Nickname.Equals(message.Target, StringComparison.OrdinalIgnoreCase));
            if (targetClient is not null)
            {
                // var from = $"{ircClient.Profile.Nickname}!{ircClient.Profile.User.UserName}@{ircClient.Address}";
                var from = ircClient.Profile.Nickname;
                await targetClient.WriteMessageAsync(new PrivMsgMessage(from, message.Target, message.Text));
            }
            else
            {
                await ircClient.WriteMessageAsync(new NoRecipientError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, $":No recipient given ({message})"));
            }
        }
    }
}
