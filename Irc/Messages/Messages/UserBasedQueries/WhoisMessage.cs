using System;
using System.Linq;
using System.Threading.Tasks;
using Irc.Helpers;
using Messages.Replies.CommandResponses;
using Messages.Replies.ErrorReplies;

namespace Irc.Messages.Messages
{
    public class WhoisMessage : Message
    {
        public string Mask { get; set; }

        public WhoisMessage(string mask = null)
        {
            Mask = mask;
        }

        public override string ToString()
        {
            return $"{Command} {Mask}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            if (string.IsNullOrEmpty(Mask))
            {
                await ircClient.WriteMessageAsync(new NoNicknameGivenError(ircClient.Profile.Nickname, NoNicknameGivenError.DefaultMessage));
                return true;
            }

            var regex = MaskHelper.GetRegex(Mask);

            var clients = IrcClient.IrcServer.IrcClients.Where(client => regex.IsMatch(client.Profile.Nickname));

            if (!clients.Any())
            {
                await ircClient.WriteMessageAsync(new NoSuchNickError(ircClient.Profile.Nickname, Mask, NoSuchNickError.DefaultMessage));
                return true;
            }

            foreach (var client in clients)
            {
                await ircClient.WriteMessageAsync(new WhoisReply(ircClient.Profile.Nickname, client.Profile.Nickname, client.Profile.User.UserName, client.Address.ToString(), client.Profile.User.RealName));
                if (ircClient.Channels.Any())
                {
                    var channelNames = ircClient.Channels.Values.Select(channel => channel.Name);
                    await ircClient.WriteMessageAsync(new WhoisChannelsReply(ircClient.Profile.Nickname, client.Profile.Nickname, channelNames));
                }
                await ircClient.WriteMessageAsync(new WhoisServerReply(ircClient.Profile.Nickname, client.Profile.Nickname, IrcClient.IrcServer.ServerName, IrcClient.IrcServer.Version.ToString()));
                // if (ircClient.Profile.IsAway)
                // {
                //     await ircClient.WriteMessageAsync(new AwayReply());
                // }
                // if (ircClient.IsOperator)
                // {
                //     await ircClient.WriteMessageAsync(new WhoisOperatorReply());
                // }
                var idle = (int)(DateTime.Now - ircClient.LastMessageDateTime).TotalSeconds;
                await ircClient.WriteMessageAsync(new WhoisIdleReply(ircClient.Profile.Nickname, client.Profile.Nickname, idle, WhoisIdleReply.DefaultMessage));
                await ircClient.WriteMessageAsync(new EndOfWhoisReply(ircClient.Profile.Nickname, client.Profile.Nickname, EndOfWhoisReply.DefaultMessage));
            }

            return true;
        }
    }
}