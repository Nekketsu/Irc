using Irc.Messages;
using System.Text.RegularExpressions;

namespace Messages.Replies.CommandResponses;

[Command(RPL_GLOBALUSERS)]
public class GlobalUsersReply : Reply
{
    const string RPL_GLOBALUSERS = "266";

    public int CurrentClientCount { get; }
    public int MaximumClientCount { get; }

    public GlobalUsersReply(string sender, string target, int currentClientCount, int maximumClientCount) : base(sender, target, RPL_GLOBALUSERS)
    {
        CurrentClientCount = currentClientCount;
        MaximumClientCount = maximumClientCount;
    }

    public override string InnerToString()
    {
        return $":Current global users {CurrentClientCount}, max {MaximumClientCount}";
    }

    public new static GlobalUsersReply Parse(string message)
    {
        var messageSplit = message.Split();

        var sender = messageSplit[0][":".Length..];
        var target = messageSplit[1];

        var text = message
[messageSplit[0].Length..].TrimStart()
[messageSplit[1].Length..].TrimStart()
[messageSplit[2].Length..].TrimStart();

        var numberRegex = new Regex(@"\d+");
        var matches = numberRegex.Matches(text);

        var currentClientCount = int.Parse(matches[0].Value);
        var maximumClientCount = int.Parse(matches[1].Value);

        return new(sender, target, currentClientCount, maximumClientCount);
    }
}