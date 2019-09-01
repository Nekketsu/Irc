using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Irc.Helpers;
using Irc.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class WhoMessage : Message
    {
        public string Mask { get; set; }

        public WhoMessage(string mask = null)
        {
            Mask = mask;
        }

        public override string ToString()
        {
            return $"{Command} {Mask}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            IEnumerable<IrcClient> clients;

            // No mask
            if (string.IsNullOrEmpty(Mask) || (Mask == "0"))
            {
                clients = IrcClient.IrcServer.IrcClients;
            }
            else
            {
                var regex = MaskHelper.GetRegex(Mask);

                // Channel mask
                if (Mask.StartsWith("#"))
                {
                    clients = IrcClient.IrcServer.Channels.Values
                        .Where(channel => regex.IsMatch(channel.Name))
                        .SelectMany(channel => channel.IrcClients.Values);
                }
                // Client mask
                else
                {
                    clients = IrcClient.IrcServer.IrcClients.Where(client =>
                        regex.IsMatch(client.Address.ToString()) ||
                        regex.IsMatch(IrcClient.IrcServer.ServerName) ||
                        regex.IsMatch(client.Profile.User.RealName) ||
                        regex.IsMatch(client.Profile.Nickname));
                }
            }
                
            foreach (var client in clients)
            {
                await ircClient.WriteMessageAsync(
                    new WhoReply(
                        ircClient.Profile.Nickname,
                        client.Channels.Values?.LastOrDefault()?.Name ?? "*",
                        client.Profile.User.UserName,
                        client.Address.ToString(),
                        IrcClient.IrcServer.ServerName,
                        client.Profile.Nickname,
                        client.Profile.User.RealName
                        ));
            }
            await ircClient.WriteMessageAsync(new EndOfWhoReply(ircClient.Profile.Nickname, Mask, EndOfWhoReply.DefaultMessage));

            return true;
        }
    }
}