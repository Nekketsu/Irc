using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_ENDOFWHO)]
    public class EndOfWhoReply : Reply
    {
        public const string DefaultMessage = "End of WHO list";

        const string RPL_ENDOFWHO = "315";
        public string Mask { get; set; }
        public string Message { get; set; }

        public EndOfWhoReply(string sender, string target, string mask, string message) : base(sender, target, RPL_ENDOFWHO)
        {
            Mask = mask;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Mask} :{Message}";
        }

        public static new EndOfWhoReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = message.Split(':').First();
            var target = messageSplit[2];
            var mask = messageSplit[3];
            var messageText = message.Split(':').Last();

            return new(sender, target, mask, messageText);
        }
    }
}