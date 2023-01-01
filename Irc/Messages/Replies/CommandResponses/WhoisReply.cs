using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_WHOISREPLY)]
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

        public new static WhoisReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var nickname = messageSplit[3];
            var user = messageSplit[4];
            var host = messageSplit[5];

            var realName = message
                .Substring(sender.Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(target.Length).TrimStart()
                .Substring(nickname.Length).TrimStart()
                .Substring(user.Length).TrimStart()
                .Substring(host.Length).TrimStart()
                .Substring(messageSplit[6].Length).TrimStart()
                .TrimStart(':'); // ':'

            return new WhoisReply(sender, target, nickname, user, host, realName);
        }
    }
}