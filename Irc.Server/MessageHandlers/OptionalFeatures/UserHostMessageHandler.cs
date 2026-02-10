using Irc.Messages.Messages;
using Messages.Replies.CommandResponses;

namespace Irc.Server.MessageHandlers.OptionalFeatures;

public class UserHostMessageHandler : MessageHandler<UserhostMessage>
{
    public async override Task<bool> HandleAsync(UserhostMessage _, IrcClient ircClient)
    {
        var message = ircClient.Profile.Nickname;
        if (ircClient.Profile.User is not null)
        {
            message += $"=+~{ircClient.Profile.User.UserName}";
        }
        message += $"@{ircClient.Address}";
        var userHost = new UserHostReply(ircClient.IrcServer.ServerName, ircClient.Profile.Nickname, $"{message}");
        await ircClient.WriteMessageAsync(userHost);

        return true;
    }
}
