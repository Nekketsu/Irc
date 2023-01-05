using System.Collections.Generic;

namespace Irc.Client.Wpf.Domain
{
    public class Channel
    {
        public string Name { get; set; }
        public Dictionary<string, User> Users { get; set; }
        public string Topic { get; set; }

        public Channel(string name)
        {
            Name = name;
            Users = new Dictionary<string, User>(NicknameEqualityComparer.Default);
        }
    }
}
