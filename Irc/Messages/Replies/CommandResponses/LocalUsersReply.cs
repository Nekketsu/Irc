using Irc.Messages;
using System.Text.RegularExpressions;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_LOCALUSERS)]
    public class LocalUsersReply : Reply
    {
        const string RPL_LOCALUSERS = "265";

        public int CurrentClientCount { get; }
        public int MaximumClientCount { get; }

        public LocalUsersReply(string sender, string target, int currentClientCount, int maximumClientCount) : base(sender, target, RPL_LOCALUSERS)
        {
            CurrentClientCount = currentClientCount;
            MaximumClientCount = maximumClientCount;
        }

        public override string InnerToString()
        {
            return $":Current local users {CurrentClientCount}, max {MaximumClientCount}";
        }

        public new static LocalUsersReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[1];

            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart();

            var numberRegex = new Regex(@"\d+");
            var matches = numberRegex.Matches(text);

            var currentClientCount = int.Parse(matches[0].Value);
            var maximumClientCount = int.Parse(matches[1].Value);

            return new(sender, target, currentClientCount, maximumClientCount);
        }
    }
}