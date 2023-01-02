using System.Collections.Generic;

namespace Irc.Client.Wpf.Domain
{
    public class Channel
    {
        public string Name { get; set; }
        public SortedDictionary<string, User> Users { get; set; }

        public Channel(string name)
        {
            Name = name;
            Users = new SortedDictionary<string, User>();
        }
    }
}
