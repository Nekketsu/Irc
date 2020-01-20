using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Server.MessageHandlers.OptionalFeatures
{
    public class UserHostMessageHandler : MessageHandler<UserHostMessage>
    {
        public async override Task<bool> HandleAsync(UserHostMessage _, IrcClient ircClient)
        {
            var message = ircClient.Profile.Nickname;
            if (ircClient.Profile.User != null)
            {
                message += $"=+~{ircClient.Profile.User.UserName}";
            }
            message += $"@{ircClient.Address}";
            var userHost = new UserHostReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, $"{message}");
            await ircClient.WriteMessageAsync(userHost);

            return true;
        }
    }
}
