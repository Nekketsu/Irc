using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_ENDOFMOTD)]
    public class EndOfMotdReply : Reply
    {
        const string RPL_ENDOFMOTD = "376";

        public const string DefaultMessage = "End of /MOTD command.";

        public string Message { get; }

        public EndOfMotdReply(string sender, string target, string message) : base(sender, target, RPL_ENDOFMOTD)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }

        public new static EndOfMotdReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];
            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(":".Length).Split()[0];

            return new(sender, target, text);
        }
    }
}