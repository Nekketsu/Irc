namespace Irc.Client.Events
{
    public class ChannelEventArgs : EventArgs
    {
        public Channel Channel { get; }

        public ChannelEventArgs(Channel channel)
        {
            Channel = channel;
        }
    }
}
