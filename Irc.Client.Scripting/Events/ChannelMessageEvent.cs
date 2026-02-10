using Irc.Client.Events;
using Irc.Client.Scripting.Api;
using Irc.Client.Scripting.Api.Events;

namespace Irc.Client.Scripting.Events;

/// <summary>
/// Represents a subscription to channel message events.
/// </summary>
public class ChannelMessageEvent : IScriptEvent
{
    private readonly Func<ChannelMessageEventArgs, Task> handler;
    private readonly Action<string> logger;
    private readonly List<ChannelSubscription> channelSubscriptions = [];
    private EventHandler<ChannelEventArgs>? channelJoinedHandler;

    public ChannelMessageEvent(Func<ChannelMessageEventArgs, Task> handler, Action<string> logger)
    {
        this.handler = handler;
        this.logger = logger;
    }

    public string EventTypeName => "ChannelMessageReceived";

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
            subscription.Channel.MessageReceived -= subscription.Handler;
        }
        channelSubscriptions.Clear();
    }

    private void SubscribeToChannel(Channel channel)
    {
        EventHandler<MessageEventArgs> wrappedHandler = async (sender, e) =>
        {
            try
            {
                var channelEventArgs = ChannelMessageEventArgs.FromEvent(channel, e.From, e.Message);
                await handler(channelEventArgs);
            }
            catch (Exception ex)
            {
                logger($"Error in ChannelMessage handler: {ex.Message}");
            }
        };

        channel.MessageReceived += wrappedHandler;
        channelSubscriptions.Add(new ChannelSubscription(channel, wrappedHandler));
    }

    private record ChannelSubscription(Channel Channel, EventHandler<MessageEventArgs> Handler);
}
