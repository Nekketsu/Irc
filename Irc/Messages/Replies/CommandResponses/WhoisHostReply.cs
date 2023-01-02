﻿namespace Irc.Messages.Replies.CommandResponses
{
    [Command(RPL_WHOISHOST)]
    public class WhoisHostReply : Reply
    {
        const string RPL_WHOISHOST = "378";

        public string Nickname { get; set; }
        public string Text { get; set; }

        public WhoisHostReply(string sender, string target, string nickname, string text) : base(sender, target, RPL_WHOISHOST)
        {
            Nickname = nickname;
            Text = text;
        }


        public override string InnerToString()
        {
            return $"{Nickname} :{Text}";
        }

        public new static WhoisHostReply Parse(string message)
        {
            var messageSplit = message.Split();

            var sender = messageSplit[0];
            var target = messageSplit[2];
            var nickname = messageSplit[3];
            var text = message
                .Substring(messageSplit[0].Length).TrimStart()
                .Substring(messageSplit[1].Length).TrimStart()
                .Substring(messageSplit[2].Length).TrimStart()
                .Substring(messageSplit[3].Length).TrimStart()
                .TrimStart(':');

            return new WhoisHostReply(sender, target, nickname, text);
        }
    }
}