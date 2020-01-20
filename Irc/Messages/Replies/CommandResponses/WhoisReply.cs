using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class WhoisReply : Reply
    {
        const string RPL_WHOISREPLY = "311";
        public string Nickname { get; set; }
        public string User { get; set; }
        public string Host { get; set; }
        public string RealName { get; set; }

        public WhoisReply(string sender, string target, string nickname, string user, string host, string realName) : base(sender, target, RPL_WHOISREPLY)
        {
            Nickname = nickname;
            User = user;
            Host = host;
            RealName = realName;
        }

        public override string InnerToString()
        {
            return $"{Nickname} {User} {Host} * :{RealName}";
        }
    }
}