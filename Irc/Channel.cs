using System;
using System.Collections.Generic;

namespace Irc
{
    public class Channel
    {
        public string Name { get; private set; }
        public Topic Topic { get; set; }
        public List<IrcClient> IrcClients { get; private set; }
        public DateTime CreationTime { get; private set; }

        public Channel(string name)
        {
            CreationTime = DateTime.Now;
            Name = name;
            IrcClients = new List<IrcClient>();
        }
    }
}