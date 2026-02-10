using Irc.Client.Events;
using Irc.Client.Scripting.Api;
using Irc.Client.Scripting.Api.Events;

namespace Irc.Client.Scripting.Events;

/// <summary>
/// Represents a subscription to user parted channel events.
/// </summary>
public class UserPartedEvent : IScriptEvent
{
    private readonly Func<ChannelUserEventArgs, Task> handler;
    private readonly Action<string> logger;
    private readonly List<ChannelSubscription> channelSubscriptions = [];
    private EventHandler<ChannelEventArgs>? channelJoinedHandler;

    public UserPartedEvent(Func<ChannelUserEventArgs, Task> handler, Action<string> logger)
    {
        this.handler = handler;
        this.logger = logger;
    }

    public string EventTypeName => "UserPartedChannel";

    public void Subscribe(IrcClient client)
    {
        // Subscribe to existing channels
        foreach (var channel in client.Channels)
        {
            SubscribeToChannel(channel);
        }

        // Subscribe to future channels
        channelJoinedHandler = (sender, e) =>
        {
            SubscribeToChannel(e.Channel);
        };

        client.LocalUser.ChannelJoined += channelJoinedHandler;
    }

    public void Unsubscribe(IrcClient client)
    {
        // Unsubscribe from channel joined event
        if (channelJoinedHandler is not null)
        {
            client.LocalUser.ChannelJoined -= channelJoinedHandler;
            channelJoinedHandler = null;
        }

        // Unsubscribe from all channels
        foreach (var subscription in channelSubscriptions)
        {
            subscription.Channel.UserLeft -= subscription.Handler;
        }
        channelSubscriptions.Clear();
    }

    private void SubscribeToChannel(Channel channel)
    {
        EventHandler<UserEventArgs> wrappedHandler = async (sender, e) =>
        {
            try
            {
                var channelEventArgs = ChannelUserEventArgs.FromEvent(channel, e.Nickname);
                await handler(channelEventArgs);
            }
            catch (Exception ex)
            {
                logger($"Error in UserParted handler: {ex.Message}");
            }
        };

        channel.UserLeft += wrappedHandler;
        channelSubscriptions.Add(new ChannelSubscription(channel, wrappedHandler));
    }

    private record ChannelSubscription(Channel Channel, EventHandler<UserEventArgs> Handler);
}
