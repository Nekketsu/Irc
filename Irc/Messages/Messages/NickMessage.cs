using System.Threading.Tasks;

namespace Irc.Messages.Messages
{
    public class NickMessage : Message
    {
        public string NickName { get; private set; }

        public NickMessage(string nickName)
        {
            NickName = nickName;
        }

        public override string ToString()
        {
            return $"{Command} {NickName}";
        }

        public override Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            ircClient.Profile.NickName = NickName;
            return Task.FromResult(true);
        }
    }
}