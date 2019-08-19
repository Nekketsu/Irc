using System.Threading.Tasks;
using Messages.Replies.CommandResponses;

namespace Irc.Messages.Messages
{
    public class TopicMessage : Message
    {
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

        public override string ToString()
        {
            var text = $"{Command} {ChannelName}";
            if (Topic != null)
            {
                text = $"{text} :{Topic}";
            }

            return text;
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
                        await ircClient.WriteMessageAsync(new NoTopicReply(ircClient.Profile.NickName, ChannelName, NoTopicReply.DefaultMessage));
                    }
                    else
                    {
                        await ircClient.WriteMessageAsync(new TopicReply(ircClient.Profile.NickName, ChannelName, channel.Topic.TopicMessage));
                        await ircClient.WriteMessageAsync(new TopicWhoTimeReply(ircClient.Profile.NickName, ChannelName, channel.Topic.NickName, channel.Topic.SetAt));
                    }
                }
                // Set topic
                else
                {
                    var topic = (Topic == string.Empty) ? null : Topic;
                    channel.Topic = new Topic(topic, ircClient.Profile.NickName);

                    var topicReply = new TopicReply(ircClient.Profile.NickName, ChannelName, channel.Topic.TopicMessage);
                    var topicWhoTimeReply = new TopicWhoTimeReply(ircClient.Profile.NickName, ChannelName, channel.Topic.NickName, channel.Topic.SetAt);
                    foreach (var client in channel.IrcClients)
                    {
                        await client.WriteMessageAsync(topicReply);
                        await client.WriteMessageAsync(topicWhoTimeReply);
                    }
                }
            }
            // No on channel
            else if (IrcClient.IrcServer.Channels.ContainsKey(ChannelName))
            {
                await ircClient.WriteMessageAsync(new NotOnChannelReply(ircClient.Profile.NickName, ChannelName, NotOnChannelReply.DefaultMessage));
            }
            // No such channel
            else
            {
                await ircClient.WriteMessageAsync(new NoSuchChannelReply(ircClient.Profile.NickName, ChannelName, NoSuchChannelReply.DefaultMessage));
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