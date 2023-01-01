using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_WHOISIDLE)]
    public class WhoisIdleReply : Reply
    {
        public const string DefaultMessage = "seconds idle";

        const string RPL_WHOISIDLE = "317";
        public string Nickname { get; set; }
        public int Idle { get; set; }
        public string Message { get; set; }

        public WhoisIdleReply(string sender, string target, string nickname, int idle, string message) : base(sender, target, RPL_WHOISIDLE)
        {
            Nickname = nickname;
            Idle = idle;
            Message = message;
        }

        public override string InnerToString()
        {
            return $"{Nickname} {Idle} :{Message}";
        }
    }
}