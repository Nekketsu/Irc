using Irc.Helpers;
using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using Messages.Replies.ErrorReplies;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Irc.Server.MessageHandlers.UserBasedQueries
{
    public class WhoisMessageHandler : MessageHandler<WhoisMessage>
    {
        public async override Task<bool> HandleAsync(WhoisMessage message, IrcClient ircClient)
        {
            if (string.IsNullOrEmpty(message.Mask))
            {
                await ircClient.WriteMessageAsync(new NoNicknameGivenError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, NoNicknameGivenError.DefaultMessage));
                return true;
            }

            var regex = MaskHelper.GetRegex(message.Mask);

            var clients = ircClient.IrcServer.IrcClients.Where(client => regex.IsMatch(client.Profile.Nickname));

            if (!clients.Any())
            {
                await ircClient.WriteMessageAsync(new NoSuchNickError(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.Mask, NoSuchNickError.DefaultMessage));
                return true;
            }

            foreach (var client in clients)
            {
                await ircClient.WriteMessageAsync(new WhoisReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, client.Profile.Nickname, client.Profile.User.UserName, client.Address.ToString(), client.Profile.User.RealName));
                if (ircClient.Channels.Any())
                {
                    var channelNames = ircClient.Channels.Values.Select(channel => channel.Name);
                    await ircClient.WriteMessageAsync(new WhoisChannelsReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, client.Profile.Nickname, channelNames));
                }
                await ircClient.WriteMessageAsync(new WhoisServerReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, client.Profile.Nickname, ircClient.IrcServer.ServerName, ircClient.IrcServer.Version.ToString()));
                // if (ircClient.Profile.IsAway)
                // {
                //     await ircClient.WriteMessageAsync(new AwayReply());
                // }
                // if (ircClient.IsOperator)
                // {
                //     await ircClient.WriteMessageAsync(new WhoisOperatorReply());
                // }
                var idle = (int)(DateTime.Now - ircClient.LastMessageDateTime).TotalSeconds;
                await ircClient.WriteMessageAsync(new WhoisIdleReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, client.Profile.Nickname, idle, WhoisIdleReply.DefaultMessage));
                await ircClient.WriteMessageAsync(new EndOfWhoisReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, client.Profile.Nickname, EndOfWhoisReply.DefaultMessage));
            }

            return true;
        }
    }
}
