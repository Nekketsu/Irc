namespace Irc.Messages.Replies
{
    [Command(RPL_WHOISOPERATOR)]
    public class WhoisOperatorReply : Reply
    {
        public const string DefaultMessage = "is an IRC operator";

        const string RPL_WHOISOPERATOR = "313";

        public string Nickname { get; set; }
        public string Message { get; set; }

        public WhoisOperatorReply(string sender, string target, string nickname, string message) : base(sender, target, RPL_WHOISOPERATOR)
        {
            Nickname = nickname;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Nickname} :{Message}";
        }

        public new static WhoisOperatorReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var nickname = messageSplit[3];
            var textMessage = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(messageSplit[3].Length).TrimStart()
                .TrimStart(':');

            return new WhoisOperatorReply(sender, target, nickname, textMessage);
        }
    }
}
