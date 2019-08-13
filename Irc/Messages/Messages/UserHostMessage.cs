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
            var userHost = new UserHostReply($"{ircClient.Profile.NickName} :Your host is {ircClient.Address}, running version {IrcClient.IrcServer.Version}");
            await ircClient.StreamWriter.WriteMessageAsync(userHost);

            return true;
        }
    }
}