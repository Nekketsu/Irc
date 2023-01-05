using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_LUSERCHANNELS)]
    public class LUserChannels : Reply
    {
        const string RPL_LUSERCHANNELS = "254";

        public int ChannelCount { get; }

        public LUserChannels(string sender, string target, int channelCount) : base(sender, target, RPL_LUSERCHANNELS)
        {
            ChannelCount = channelCount;
        }

        public override string InnerToString()
        {
            return $"{ChannelCount} :channels formed";
        }

        public new static LUserChannels Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0].Substring(":".Length);
            var target = messageSplit[2];
            var channelCount = int.Parse(messageSplit[3]);

            return new(sender, target, channelCount);
        }
    }
}