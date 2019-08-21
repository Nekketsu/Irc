using System.Threading.Tasks;
using Irc.Extensions;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class UserHostMessage : Message
    {
        public string Nickname { get; set; }

        public UserHostMessage(string nickname)
        {
            Nickname = nickname;
        }

        public override string ToString()
        {
            return $"{Command} {Nickname}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            var message = ircClient.Profile.Nickname;
            if (ircClient.Profile.User != null)
            {
                message += $"=+~{ircClient.Profile.User.UserName}";
            }
            message += $"@{ircClient.Address}";
            var userHost = new UserHostReply(ircClient.Profile.Nickname, $"{message}");
            await ircClient.WriteMessageAsync(userHost);

            return true;
        }
    }
}