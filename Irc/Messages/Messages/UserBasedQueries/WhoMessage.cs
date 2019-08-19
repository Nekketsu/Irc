using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Irc.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class WhoMessage : Message
    {
        public string Mask { get; set; }

        public WhoMessage(string mask)
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
            if ((Mask == null) || (Mask == "0"))
            {
                clients = IrcClient.IrcServer.IrcClients;
            }
            else
            {
                var regex = new Regex(GetPattern(Mask));

                // Channel mask
                if (Mask.StartsWith("#"))
                {
                    clients = IrcClient.IrcServer.Channels.Values
                        .Where(channel => regex.IsMatch(channel.Name))
                        .SelectMany(channel => channel.IrcClients);
                }
                // Client mask
                else
                {
                    clients = IrcClient.IrcServer.IrcClients.Where(client =>
                        regex.IsMatch(client.Address.ToString()) ||
                        regex.IsMatch(IrcClient.IrcServer.ServerName) ||
                        regex.IsMatch(client.Profile.User.RealName) ||
                        regex.IsMatch(client.Profile.NickName));
                }
            }
                
            foreach (var client in clients)
            {
                await ircClient.WriteMessageAsync(
                    new WhoReply(
                        ircClient.Profile.NickName,
                        client.Channels.Values?.LastOrDefault()?.Name ?? "*",
                        client.Profile.User.UserName,
                        client.Address.ToString(),
                        IrcClient.IrcServer.ServerName,
                        client.Profile.NickName,
                        client.Profile.User.RealName
                        ));
            }
            await ircClient.WriteMessageAsync(new EndOfWhoReply(ircClient.Profile.NickName, Mask, EndOfWhoReply.DefaultMessage));

            return true;
        }

        private string GetPattern(string mask)
        {
            var pattern = Regex.Escape(mask);
            pattern = pattern.Replace("\\*", ".*");
            pattern = pattern.Replace("\\?", ".");

            return pattern;
        }
    }
}