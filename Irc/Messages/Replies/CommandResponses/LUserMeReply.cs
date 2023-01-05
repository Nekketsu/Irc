using Irc.Messages;
using System.Text.RegularExpressions;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_LUSERME)]
    public class LUserMeReply : Reply
    {
        const string RPL_LUSERME = "255";

        public int ClientCount { get; }
        public int ServerCount { get; }

        public LUserMeReply(string sender, string target, int clientCount, int serverCount) : base(sender, target, RPL_LUSERME)
        {
            ClientCount = clientCount;
            ServerCount = serverCount;
        }

        public override string InnerToString()
        {
            return $":I have {ClientCount} clients and {ServerCount} servers";
        }

        public new static LUserMeReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];

            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart();

            var numberRegex = new Regex(@"\d+");
            var matches = numberRegex.Matches(text);

            var clientCount = int.Parse(matches[0].Value);
            var serverCount = int.Parse(matches[1].Value);

            return new(sender, target, clientCount, serverCount);
        }
    }
}