using System.Threading.Tasks;

namespace Irc.Messages.Messages
{
    public class NickMessage : Message
    {
        public string Nickname { get; private set; }

        public NickMessage(string nickname)
        {
            Nickname = nickname;
        }

        public override string ToString()
        {
            return $"{Command} {Nickname}";
        }

        public override Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            ircClient.Profile.Nickname = Nickname;
            return Task.FromResult(true);
        }
    }
}