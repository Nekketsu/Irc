using System.Threading.Tasks;
using Irc.Extensions;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class UserHostMessage : Message
    {
        public string NickName { get; set; }

        public UserHostMessage(string nickName)
        {
            NickName = nickName;
        }

        public override string ToString()
        {
            return $"{Command} {NickName}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            var message = ircClient.Profile.NickName;
            if (ircClient.Profile.User != null)
            {
                message += $"=+~{ircClient.Profile.User.UserName}";
            }
            message += $"@{ircClient.Address}";
            var userHost = new UserHostReply(ircClient.Profile.NickName, $"{message}");
            await ircClient.WriteMessageAsync(userHost);

            return true;
        }
    }
}