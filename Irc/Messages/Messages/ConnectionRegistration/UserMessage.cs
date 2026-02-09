namespace Irc.Messages.Messages
{
    [Command("USER")]
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

            var realName = message.Split(':').Last();

            return new UserMessage(user, mode, unused, realName);
        }
    }
}