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
            return $"{Command} {User} {Mode} {Unused} :{RealName}";
        }

        public new static UserMessage Parse(string message)
        {
            var messageSplit = message.Split();

            var user = messageSplit[1];
            var mode = messageSplit[2];
            var unused = messageSplit[3];

            var realName = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            realName = message.Substring(realName.IndexOf(messageSplit[1]) + messageSplit[1].Length).TrimStart();
            realName = message.Substring(realName.IndexOf(messageSplit[2]) + messageSplit[1].Length).TrimStart();
            realName = message.Substring(realName.IndexOf(messageSplit[3]) + messageSplit[1].Length).TrimStart();
            if (realName.StartsWith(':'))
            {
                realName = realName.Substring(1);
            }

            return new UserMessage(user, mode, unused, realName);
        }
    }
}