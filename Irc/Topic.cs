using System;

namespace Irc
{
    public class Topic
    {
        public string TopicMessage { get; private set; }
        public string Nickname { get; set; }
        public DateTime SetAt { get; private set; }

        public Topic(string topicMessage, string nickname)
        {
            TopicMessage = topicMessage;
            Nickname = nickname;
            SetAt = DateTime.Now;
        }
    }
}