using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_WELCOME)]
    public class WelcomeReply : Reply
    {
        const string RPL_WELCOME = "001";

        public string NetworkName { get; }
        public string Nickname { get; }

        public WelcomeReply(string sender, string target, string networkName, string nickname) : base(sender, target, RPL_WELCOME)
        {
            NetworkName = networkName;
            Nickname = nickname;
        }

        public override string InnerToString()
        {
            return $":Welcome to the {NetworkName} Network, {Nickname}";
        }

        public new static WelcomeReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var networkName = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(":Welcome to the ".Length).Split()[0];
            var nickname = messageSplit.Last();

            return new(sender, target, networkName, nickname);
        }
    }
}