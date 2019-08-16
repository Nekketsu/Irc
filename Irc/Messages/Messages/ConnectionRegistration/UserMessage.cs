using System.Threading.Tasks;

namespace Irc.Messages.Messages
{
    public class UserMessage : Message
    {
        public string User { get; private set; }
        public string Mode { get; private set; }
        public string Unused { get; private set; }
        public string RealName { get; private set; }

        public UserMessage(string user, string mode, string unused, string realName)
        {
            User = user;
            Mode = mode;
            Unused = unused;
            RealName = realName;
        }

        public override string ToString()
        {
            return $"{Command} {User} {Mode} {Unused} {RealName}";
        }

        public override Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            ircClient.Profile.User = new User(User, Mode, Unused, RealName);
            return Task.FromResult(true);
        }
    }
}