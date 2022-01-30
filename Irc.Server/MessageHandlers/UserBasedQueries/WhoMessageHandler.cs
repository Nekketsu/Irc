using Irc.Helpers;
using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Irc.Server.MessageHandlers.UserBasedQueries
{
    public class WhoMessageHandler : MessageHandler<WhoMessage>
    {
        public async override Task<bool> HandleAsync(WhoMessage message, IrcClient ircClient)
        {
            IEnumerable<IrcClient> clients;

            // No mask
            if (string.IsNullOrEmpty(message.Mask) || (message.Mask == "0"))
            {
                clients = ircClient.IrcServer.IrcClients;
            }
            else
            {
                var regex = MaskHelper.GetRegex(message.Mask);

                // Channel mask
                if (message.Mask.StartsWith("#"))
                {
                    clients = ircClient.IrcServer.Channels.Values
                        .Where(channel => regex.IsMatch(channel.Name))
                        .SelectMany(channel => channel.IrcClients.Values);
                }
                // Client mask
                else
                {
                    clients = ircClient.IrcServer.IrcClients.Where(client =>
                        regex.IsMatch(client.Address.ToString()) ||
                        regex.IsMatch(ircClient.IrcServer.ServerName) ||
                        regex.IsMatch(client.Profile.User.RealName) ||
                        regex.IsMatch(client.Profile.Nickname));
                }
            }

            foreach (var client in clients)
            {
                await ircClient.WriteMessageAsync(
                    new WhoReply(
                        ircClient.IrcServer.ServerName,
                        ircClient.Profile.Nickname,
                        client.Channels.Values?.LastOrDefault()?.Name ?? "*",
                        client.Profile.User.UserName,
                        client.Address.ToString(),
                        ircClient.IrcServer.ServerName,
                        client.Profile.Nickname,
                        client.Profile.User.RealName
                        ));
            }
            await ircClient.WriteMessageAsync(new EndOfWhoReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.Mask, EndOfWhoReply.DefaultMessage));

            return true;
        }
    }
}
