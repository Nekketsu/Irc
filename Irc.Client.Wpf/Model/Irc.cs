using System.Collections.Generic;
using System.Linq;

namespace Irc.Client.Wpf.Model
{
    public class Irc
    {
        public Dictionary<string, Channel> Channels { get; set; }
        public Dictionary<string, User> Users { get; set; }

        public Irc()
        {
            Channels = new Dictionary<string, Channel>();
            Users = new Dictionary<string, User>();
        }

        public void Join(string channelName, params string[] nicknames)
        {
            foreach (var nickname in nicknames)
            {
                if (!Users.TryGetValue(nickname, out var user))
                {
                    user = new User(nickname);
                    Users.Add(nickname, user);
                }

                if (!Channels.TryGetValue(channelName, out var channel))
                {
                    channel = new Channel(channelName);
                    Channels.Add(channelName, channel);
                }

                user.Channels[channel.Name] = channel;
                channel.Users[user.Name] = user;
            }
        }

        public void Part(string channelName, string nickname)
        {
            var channel = Channels[channelName];
            var user = Users[nickname];

            channel.Users.Remove(nickname);
            user.Channels.Remove(channelName);

            if (!user.Channels.Any())
            {
                Users.Remove(nickname);
            }
        }

        public void Quit(string nickname)
        {
            if (!Users.TryGetValue(nickname, out var user))
            {
                return;
            }

            foreach (var channel in user.Channels.Values)
            {
                channel.Users.Remove(nickname);
            }

            Users.Remove(nickname);
        }
    }
}
