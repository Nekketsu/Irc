using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Server.MessageHandlers.ChannelOperations
{
    public class ChannelModeMessageHandler : MessageHandler<ChannelModeMessage>
    {
        public async override Task<bool> HandleAsync(ChannelModeMessage message, IrcClient ircClient)
        {
            await ircClient.WriteMessageAsync(new ChannelModeIsReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, message.ChannelName, "+nt"));

            return true;
        }
    }
}
