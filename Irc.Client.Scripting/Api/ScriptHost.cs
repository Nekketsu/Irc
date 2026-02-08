using Irc.Client.Scripting.Api.Events;
using Irc.Client.Scripting.Events;
using Irc.Messages.Messages;

namespace Irc.Client.Scripting.Api
{
    /// <summary>
    /// Provides API for scripts to interact with IRC client.
    /// Scripts subscribe to events through On* methods.
    /// </summary>
    public class ScriptHost
    {
        private readonly IrcClient client;
        private readonly List<IScriptEvent> subscriptions = new();
        private readonly ILocalUserInfo me;
        private readonly Action<string>? logAction;

        public ScriptHost(IrcClient client, Action<string>? logAction = null)
        {
            this.client = client;
            this.me = new LocalUserInfo(client.LocalUser);
            this.logAction = logAction;
        }

        /// <summary>
        /// Gets read-only information about the local user and their channels.
        /// </summary>
        public ILocalUserInfo Me => me;

        // Event subscription API - creates event objects that manage their own subscriptions

        public void OnPrivateMessage(Func<PrivateMessageEventArgs, Task> handler)
        {
            var eventSubscription = new PrivateMessageEvent(handler, Log);
            eventSubscription.Subscribe(client);
            subscriptions.Add(eventSubscription);
        }

        public void OnChannelMessage(Func<ChannelMessageEventArgs, Task> handler)
        {
            var eventSubscription = new ChannelMessageEvent(handler, Log);
            eventSubscription.Subscribe(client);
            subscriptions.Add(eventSubscription);
        }

        public void OnUserJoined(Func<ChannelUserEventArgs, Task> handler)
        {
            var eventSubscription = new UserJoinedEvent(handler, Log);
            eventSubscription.Subscribe(client);
            subscriptions.Add(eventSubscription);
        }

        public void OnUserParted(Func<ChannelUserEventArgs, Task> handler)
        {
            var eventSubscription = new UserPartedEvent(handler, Log);
            eventSubscription.Subscribe(client);
            subscriptions.Add(eventSubscription);
        }

        // Cleanup method to unsubscribe from all events
        internal void UnsubscribeAll()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Unsubscribe(client);
            }
            subscriptions.Clear();
        }

        // Get subscribed event type names for display
        internal List<string> GetSubscribedEventTypes()
        {
            return subscriptions
                .Select(s => s.EventTypeName)
                .Distinct()
                .ToList();
        }

        // Action methods
        public async Task SendPrivateMessageAsync(string target, string message)
        {
            await client.SendMessageAsync(new PrivMsgMessage(target, message));
        }

        public async Task SendChannelMessageAsync(string channel, string message)
        {
            await client.SendMessageAsync(new PrivMsgMessage(channel, message));
        }

        public async Task JoinChannelAsync(string channel)
        {
            await client.SendMessageAsync(new JoinMessage(channel));
        }

        public async Task PartChannelAsync(string channel, string? reason = null)
        {
            if (reason is null)
            {
                await client.SendMessageAsync(new PartMessage(channel));
            }
            else
            {
                await client.SendMessageAsync(new PartMessage(channel, reason));
            }
        }

        public void Log(string message)
        {
            if (logAction is not null)
            {
                logAction(message);
            }
            else
            {
                Console.WriteLine($"[SCRIPT] {message}");
            }
        }
    }
}