using Irc.Extensions;
using Irc.Messages;
using System;

namespace Messages.Replies.CommandResponses
{
    public class CreationTimeReply : Reply
    {
        const string RPL_CREATIONTIME = "329";
        public string ChannelName { get; private set; }
        public DateTime CreationTime { get; private set; }

        public CreationTimeReply(string sender, string target, string channelName, DateTime creationTime) : base(sender, target, RPL_CREATIONTIME)
        {
            ChannelName = channelName;
            CreationTime = creationTime;
        }

        public override string InnerToString()
        {
            return $"{ChannelName} :{CreationTime.ToUnixTime()}";
        }
    }
}