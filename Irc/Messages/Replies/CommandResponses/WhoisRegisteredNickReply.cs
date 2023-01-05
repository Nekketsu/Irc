namespace Irc.Messages.Replies.CommandResponses
{
    [Command(RPL_WHOISREGNICK)]
    public class WhoisRegisteredNickReply : Reply
    {
        const string RPL_WHOISREGNICK = "307";

        public string Nickname { get; }
        public string Text { get; }

        public WhoisRegisteredNickReply(string sender, string target, string nickname, string text) : base(sender, target, RPL_WHOISREGNICK)
        {
            Nickname = nickname;
            Text = text;
        }

        public override string InnerToString()
        {
            return $"{Nickname} :{Text}";
        }

        public new static WhoisRegisteredNickReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];
            var nickname = messageSplit[3];
            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(messageSplit[3].Length).TrimStart()
                .TrimStart(':');

            return new WhoisRegisteredNickReply(sender, target, nickname, text);
        }
    }
}
