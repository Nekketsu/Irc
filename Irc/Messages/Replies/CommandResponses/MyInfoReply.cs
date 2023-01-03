using Irc.Messages;

namespace Messages.Replies.CommandResponses
{
    [Command(RPL_MYINFO)]
    public class MyInfoReply : Reply
    {
        const string RPL_MYINFO = "004";

        public string ServerName { get; }
        public string Version { get; }
        public string AvailableUserModes { get; }
        public string AvailableChannelModes { get; }
        public string ChannelModesWithAParameter { get; }

        public MyInfoReply(string sender, string target, string serverName, string version, string availableUserModes, string availableChannelModes, string channelModesWithAParameter) : base(sender, target, RPL_MYINFO)
        {
            ServerName = serverName;
            Version = version;
            AvailableUserModes = availableUserModes;
            AvailableChannelModes = availableChannelModes;
            ChannelModesWithAParameter = channelModesWithAParameter;
        }

        public override string InnerToString()
        {
            return $":{ServerName} {Version} {AvailableUserModes} {AvailableChannelModes} :{ChannelModesWithAParameter}";
        }

        public new static MyInfoReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var serverName = messageSplit[3];
            var version = messageSplit[4];
            var availableUserModes = messageSplit[4];
            var availableChannelModes = messageSplit[4];
            var channelModesWithAParameter = messageSplit[4].Substring(":".Length);

            return new(sender, target, serverName, version, availableUserModes, availableChannelModes, channelModesWithAParameter);
        }
    }
}