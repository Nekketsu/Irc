using Irc.Messages;
using System.Text.RegularExpressions;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_LUSERCLIENT)]
    public class LUserClientReply : Reply
    {
        const string RPL_LUSERCLIENT = "251";

        public int TotalUserCount { get; }
        public int InvisibleUserCount { get; }
        public int SeverCount { get; }

        public LUserClientReply(string sender, string target, int totalUserCount, int invisibleUserCount, int severCount) : base(sender, target, RPL_LUSERCLIENT)
        {
            TotalUserCount = totalUserCount;
            InvisibleUserCount = invisibleUserCount;
            SeverCount = severCount;
        }

        public override string InnerToString()
        {
            return $":There are {TotalUserCount} users and {InvisibleUserCount} invisible on {SeverCount} servers";
        }

        public new static LUserClientReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];

            var numberRegex = new Regex(@"\d+");
            var matches = numberRegex.Matches(message);

            var totalUserCount = int.Parse(matches[1].Value);
            var invisibleUserCount = int.Parse(matches[2].Value);
            var serverCount = int.Parse(matches[3].Value);

            return new(sender, target, totalUserCount, invisibleUserCount, serverCount);
        }
    }
}