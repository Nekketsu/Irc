﻿using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Server.MessageHandlers.ChannelOperations
{
    public class NamesMessageHandler : MessageHandler<NamesMessage>
    {
        public async override Task<bool> HandleAsync(NamesMessage message, IrcClient ircClient)
        {
            if (ircClient.IrcServer.Channels.TryGetValue(message.ChannelName, out var channel))
            {
                var nicknames = channel.IrcClients.Values.Select(client => client.Profile.Nickname).ToArray();
                await ircClient.WriteMessageAsync(new NameReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, channel.Name, nicknames));
                await ircClient.WriteMessageAsync(new EndOfNamesReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, channel.Name, EndOfNamesReply.DefaultMessage));
            }

            return true;
        }
    }
}
