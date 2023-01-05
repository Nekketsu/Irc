namespace Irc.Messages.Replies.CommandResponses
{
    [Command(RPL_UNAWAY)]
    public class UnAwayReply : Reply
    {
        const string RPL_UNAWAY = "305";

        public string Message { get; set; }

        public UnAwayReply(string sender, string target, string message) : base(sender, target, RPL_UNAWAY)
        {
            Message = message;
        }


        public override string InnerToString()
        {
            return $": {Message}";
        }

        public new static UnAwayReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];
            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .TrimStart(':');

            return new UnAwayReply(sender, target, text);
        }
    }
}
