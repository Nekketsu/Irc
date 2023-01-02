namespace Irc.Messages.Replies.CommandResponses
{
    [Command(RPL_AWAY)]
    public class AwayReply : Reply
    {
        const string RPL_AWAY = "301";

        public string Nickname { get; set; }
        public string Text { get; set; }

        public AwayReply(string sender, string target, string nickname, string text) : base(sender, target, RPL_AWAY)
        {
            Nickname = nickname;
            Text = text;
        }

        public override string InnerToString()
        {
            return $"{Nickname} :{Text}";
        }

        public new static AwayReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var nickname = messageSplit[3];
            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(messageSplit[3].Length).TrimStart()
                .TrimStart(':');

            return new AwayReply(sender, target, nickname, text);
        }
    }
}
