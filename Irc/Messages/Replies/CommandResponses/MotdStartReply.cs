using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_MOTDSTART)]
    public class MotdStartReply : Reply
    {
        const string RPL_MOTDSTART = "375";

        public string Server { get; }

        public MotdStartReply(string sender, string target, string server) : base(sender, target, RPL_MOTDSTART)
        {
            Server = server;
        }

        public override string InnerToString()
        {
            return $":{Server} message of the day";
        }

        public new static MotdStartReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];
            var server = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(":".Length);

            return new(sender, target, server);
        }
    }
}