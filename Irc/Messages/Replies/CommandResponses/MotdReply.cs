using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_MOTD)]
    public class MotdReply : Reply
    {
        const string RPL_MOTD = "372";

        public string Message { get; }

        public MotdReply(string sender, string target, string message) : base(sender, target, RPL_MOTD)
        {
            Message = message;
        }

        public override string InnerToString()
        {
            return $":{Message}";
        }

        public new static MotdReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];
            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(":".Length);

            return new(sender, target, text);
        }
    }
}