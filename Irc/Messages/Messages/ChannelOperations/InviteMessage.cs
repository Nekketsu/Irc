using System;
using System.Linq;
using System.Threading.Tasks;
using Messages.Replies.CommandResponses;
using Messages.Replies.ErrorReplies;

namespace Irc.Messages.Messages
{
    public class InviteMessage : Message
    {
        public string From { get; set; }
        public string Nickname { get; set; }
        public string ChannelName { get; set; }

        public InviteMessage(string nickname, string channelName)
        {
            Nickname = nickname;
            ChannelName = channelName;
        }

        public InviteMessage(string from, string nickname, string channelName) : this(nickname, channelName)
        {
            From = from;
        }

        public override string ToString()
        {
            return From == null
                ? $"{Command} {Nickname} {ChannelName}"
                : $":{From} {Command} {Nickname} {ChannelName}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            var invitedClient = IrcClient.IrcServer.IrcClients.SingleOrDefault(client => client.Profile.Nickname.Equals(Nickname, StringComparison.OrdinalIgnoreCase));
            if (invitedClient == null)
            {
                await ircClient.WriteMessageAsync(new NoSuchNickError(ircClient.Profile.Nickname, Nickname, NoSuchNickError.DefaultMessage));
                return true;
            }
            if (!ircClient.Channels.ContainsKey(ChannelName))
            {
                await ircClient.WriteMessageAsync(new NotOnChannelError(ircClient.Profile.Nickname, ChannelName, NotOnChannelError.DefaultMessage));
                return true;
            }
            if (invitedClient.Channels.ContainsKey(ChannelName))
            {
                await ircClient.WriteMessageAsync(new UserOnChannelError(ircClient.Profile.Nickname, invitedClient.Profile.Nickname, ChannelName, UserOnChannelError.DefaultMessage));
                return true;
            }
            
            await invitedClient.WriteMessageAsync(new InviteMessage(ircClient.Profile.Nickname, Nickname, ChannelName));

            await ircClient.WriteMessageAsync(new InvitingReply(ircClient.Profile.Nickname, ChannelName, Nickname));
            // if (invitedClient.IsAway)
            // {
            //     await ircClient.WriteMessageAsync(new AwayReply());
            // }



            return true;
        }
    }
}