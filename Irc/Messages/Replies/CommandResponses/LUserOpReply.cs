using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_LUSEROP)]
    public class LUserOpReply : Reply
    {
        const string RPL_LUSEROP = "252";

        public int IrcOperatorsCount { get; }

        public LUserOpReply(string sender, string target, int ircOperatorsCount) : base(sender, target, RPL_LUSEROP)
        {
            IrcOperatorsCount = ircOperatorsCount;
        }

        public override string InnerToString()
        {
            return $"{IrcOperatorsCount} :operator(s) online";
        }

        public new static LUserOpReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var ircOperatorsCount = int.Parse(messageSplit[3]);

            return new(sender, target, ircOperatorsCount);
        }
    }
}