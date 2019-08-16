using System;
using Irc.Extensions;
using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    public class TopicWhoTimeReply : Reply
    {
        const string RPL_TOPICWHOTIME = "333";
        public string ChannelName { get; private set; }
        public string NickName { get; set; }
        public DateTime SetAt { get; set; }

        public TopicWhoTimeReply(string target, string channelName, string nickName, DateTime setAt) : base(target, RPL_TOPICWHOTIME)
        {
            ChannelName = channelName;
            NickName = nickName;
            SetAt = setAt;

        }

        public override string InnerToString()
        {
            return $"{ChannelName} {NickName} {SetAt.ToUnixTime()}";
        }
    }
}