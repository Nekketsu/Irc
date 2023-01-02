using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_WHOISIDLE)]
    public class WhoisIdleReply : Reply
    {
        public const string DefaultMessage = "seconds idle";

        const string RPL_WHOISIDLE = "317";
        public string Nickname { get; set; }
        public TimeSpan Idle { get; set; }
        public DateTime SignOn { get; set; }
        public string Message { get; set; }

        public WhoisIdleReply(string sender, string target, string nickname, TimeSpan idle, DateTime signOn, string message) : base(sender, target, RPL_WHOISIDLE)
        {
            Nickname = nickname;
            Idle = idle;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Nickname} {Idle} :{Message}";
        }

        public new static WhoisIdleReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var nickname = messageSplit[3];
            var idleString = messageSplit[4];
            var signOnString = messageSplit[5];
            var serverInfo = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(messageSplit[3].Length).TrimStart()
                .Substring(messageSplit[4].Length).TrimStart()
                .Substring(messageSplit[5].Length).TrimStart()
                .TrimStart(':');

            var idleSeconds = long.Parse(idleString);
            var idle = TimeSpan.FromSeconds(idleSeconds);

            var signOnSeconds = long.Parse(signOnString);
            var signOn = DateTimeOffset.FromUnixTimeSeconds(signOnSeconds).DateTime;

            return new WhoisIdleReply(sender, target, nickname, idle, signOn, message);
        }
    }
}