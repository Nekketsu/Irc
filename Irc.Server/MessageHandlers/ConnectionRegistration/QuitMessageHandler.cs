using Irc.Messages.Messages;
using Microsoft.Extensions.Logging;

namespace Irc.Server.MessageHandlers.ConnectionRegistration;

public class QuitMessageHandler : MessageHandler<QuitMessage>
{
    public async override Task<bool> HandleAsync(QuitMessage message, IrcClient ircClient)
    {
        var quitMessage = new QuitMessage(ircClient.Profile.Nickname, $"Quit: {message.Reason}");
        foreach (var channel in ircClient.Channels.Values)
        {
            foreach (var client in channel.IrcClients.Values)
            {
                await client.WriteMessageAsync(quitMessage);
            }

            channel.IrcClients.Remove(ircClient.Profile.Nickname);
            ircClient.Channels.Remove(channel.Name);
        }

        ircClient.Logger.LogInformation($"{ircClient.Profile.Nickname} has disconnected IRC.");

        return false;
    }
}
