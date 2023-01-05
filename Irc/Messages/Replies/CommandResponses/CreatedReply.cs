using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_CREATED)]
    public class CreatedReply : Reply
    {
        const string RPL_CREATED = "003";

        public DateTime DateTime { get; }

        public CreatedReply(string sender, string target, DateTime dateTime) : base(sender, target, RPL_CREATED)
        {
            DateTime = dateTime;
        }

        public override string InnerToString()
        {
            return $":This server was created {DateTime}";
        }

        public new static CreatedReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];
            var dateTimeText = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(":This server was created ".Length);

            var dateTime = DateTime.Parse(dateTimeText);

            return new(sender, target, dateTime);
        }
    }
}