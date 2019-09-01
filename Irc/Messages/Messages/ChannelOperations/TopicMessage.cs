using System.Threading.Tasks;
using Messages.Replies.CommandResponses;
using Messages.Replies.ErrorReplies;

namespace Irc.Messages.Messages
{
    public class TopicMessage : Message
    {
        public string From { get; set; }
        public string ChannelName { get; set; }
        public string Topic { get; set; }

        public TopicMessage(string channelName)
        {
            ChannelName = channelName;
        }

        public TopicMessage(string channelName, string topic) : this(channelName)
        {
            Topic = topic;
        }

        public TopicMessage(string from, string channelName, string topic) : this(channelName, topic)
        {
            From = from;
        }
        

        public override string ToString()
        {
            return Topic == null
                ? $"{Command} {ChannelName}"
                : From == null
                    ? $"{Command} {ChannelName} :{Topic}"
                    : $":{From} {Command} {ChannelName} :{Topic}";
        }

        public override async Task<bool> ManageMessageAsync(IrcClient ircClient)
        {
            if (ircClient.Channels.TryGetValue(ChannelName, out var channel))
            {
                // Query topic
                if (Topic == null)
                {
                    if (channel.Topic == null)
                    {
                        await ircClient.WriteMessageAsync(new NoTopicReply(ircClient.Profile.Nickname, ChannelName, NoTopicReply.DefaultMessage));
                    }
                    else
                    {
                        await ircClient.WriteMessageAsync(new TopicReply(ircClient.Profile.Nickname, ChannelName, channel.Topic.TopicMessage));
                        await ircClient.WriteMessageAsync(new TopicWhoTimeReply(ircClient.Profile.Nickname, ChannelName, channel.Topic.Nickname, channel.Topic.SetAt));
                    }
                }
                // Set topic
                else
                {
                    var topic = (Topic == string.Empty) ? null : Topic;
                    channel.Topic = new Topic(topic, ircClient.Profile.Nickname);
                    
                    var topicMessage = new TopicMessage(ircClient.Profile.Nickname, ChannelName, Topic);
                    foreach (var client in channel.IrcClients.Values)
                    {
                        await client.WriteMessageAsync(topicMessage);
                    }
                }
            }
            // No on channel
            else if (IrcClient.IrcServer.Channels.ContainsKey(ChannelName))
            {
                await ircClient.WriteMessageAsync(new NotOnChannelError(ircClient.Profile.Nickname, ChannelName, NotOnChannelError.DefaultMessage));
            }
            // No such channel
            else
            {
                await ircClient.WriteMessageAsync(new NoSuchChannelError(ircClient.Profile.Nickname, ChannelName, NoSuchChannelError.DefaultMessage));
            }

            return true;
        }

        public new static TopicMessage Parse(string message)
        {
            var messageSplit = message.Split();
            var channelName = messageSplit[1];

            var text = message.Substring(message.IndexOf(messageSplit[0]) + messageSplit[0].Length).TrimStart();
            text = message.Substring(message.IndexOf(messageSplit[1]) + messageSplit[1].Length).TrimStart();
            if (text.StartsWith(':'))
            {
                var topic = text.Substring(1);
                return new TopicMessage(channelName, topic);
            }

            return new TopicMessage(channelName);
        }
    }
}