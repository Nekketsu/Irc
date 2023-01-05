using System.Collections.Generic;

namespace Irc.Client.Wpf.Domain
{
    public class User
    {
        public string Name { get; set; }
        public Dictionary<string, Channel> Channels { get; set; }

        public User(string name)
        {
            Name = name;
            Channels = new Dictionary<string, Channel>(ChannelNameEqualityComparer.Default);
        }
    }
}
