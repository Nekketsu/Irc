using System.Collections.Generic;
using System.Linq;

namespace Irc.Client.Wpf.Domain
{
    public class Irc
    {
        public Dictionary<string, Channel> Channels { get; }
        public Dictionary<string, User> Users { get; }

        public Irc()
        {
            Channels = new Dictionary<string, Channel>(ChannelNameEqualityComparer.Default);
            Users = new Dictionary<string, User>(NicknameEqualityComparer.Default);
        }

        public void Connect(string nickname)
        {
            Users.Add(nickname, new User(nickname));
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
            channel.Users.Remove(nickname);

            //if (!Users.TryGetValue(nickname, out var user))
            //{
            //    return;
            //}

            var user = Users[nickname];

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

        public bool UserIsInChannel(string nickname, string channelName)
        {
            if (!Users.TryGetValue(nickname, out var user))
            {
                return false;
            }

            return user.Channels.ContainsKey(channelName);
        }

        public string GetNickName(string target) => target.Split('!')[0];

        public void RenameUser(string previousNickname, string nickname)
        {
            var user = Users[previousNickname];
            Users.Remove(previousNickname);

            user.Name = nickname;
            Users.Add(nickname, user);

            foreach (var channel in Channels.Values)
            {
                if (channel.Users.ContainsKey(previousNickname))
                {
                    channel.Users.Remove(previousNickname);
                    channel.Users.Add(nickname, user);
                }
            }
        }

        public bool IsChannel(string target)
        {
            return target.StartsWith("#");
        }

        public void Topic(string channelName, string topic)
        {
            Channels[channelName].Topic = topic;
        }

        public string[] GetUserByChannelName(string channelName) => Channels[channelName].Users.Keys.Order(NicknameComparer.Default).ToArray();
    }
}
