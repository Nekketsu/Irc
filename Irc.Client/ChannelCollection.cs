using System.Collections;

namespace Irc.Client
{
    public class ChannelCollection : IEnumerable<Channel>
    {
        private Dictionary<string, Channel> channels;

        public ChannelCollection()
        {
            channels = new Dictionary<string, Channel>(ChannelNameEqualityComparer.Default);
        }
        public Channel this[string channelName]
        {
            get => channels.GetValueOrDefault(channelName);
            set => channels[channelName] = value;
        }

        public IEnumerator<Channel> GetEnumerator()
        {
            return channels.Values.GetEnumerator();
        }

        internal bool Remove(string channelName)
        {
            return channels.Remove(channelName);
        }

        public bool Contains(string channelName)
        {
            return channels.ContainsKey(channelName);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
