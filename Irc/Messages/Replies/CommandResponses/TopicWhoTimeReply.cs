using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_TOPICWHOTIME)]
    public class TopicWhoTimeReply : Reply
    {
        const string RPL_TOPICWHOTIME = "333";
        public string ChannelName { get; private set; }
        public string Nickname { get; set; }
        public DateTime SetAt { get; set; }

        public TopicWhoTimeReply(string sender, string target, string channelName, string nickname, DateTime setAt) : base(sender, target, RPL_TOPICWHOTIME)
        {
            ChannelName = channelName;
            Nickname = nickname;
            SetAt = setAt;

        }

        public override string InnerToString()
        {
            return $"{ChannelName} {Nickname} {new DateTimeOffset(SetAt).ToUnixTimeSeconds()}";
        }

        public new static TopicWhoTimeReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].TrimStart(':');
            var target = messageSplit[2];
            var channelName = messageSplit[3];
            var nickname = messageSplit[4];
            var setAsString = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(messageSplit[3].Length).TrimStart()
                .Substring(messageSplit[4].Length).TrimStart()
                .TrimStart(':');

            var setAsLong = long.Parse(setAsString);
            var setAs = DateTimeOffset.FromUnixTimeSeconds(setAsLong).LocalDateTime;

            return new TopicWhoTimeReply(sender, target, channelName, nickname, setAs);
        }
    }
}