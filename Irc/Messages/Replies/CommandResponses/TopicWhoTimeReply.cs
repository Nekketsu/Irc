using System;
using Irc.Extensions;
using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
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
            return $"{ChannelName} {Nickname} {SetAt.ToUnixTime()}";
        }
    }
}