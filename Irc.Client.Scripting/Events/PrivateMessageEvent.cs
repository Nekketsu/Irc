using Irc.Client.Events;
using Irc.Client.Scripting.Api;
using Irc.Client.Scripting.Api.Events;

namespace Irc.Client.Scripting.Events
{
    /// <summary>
    /// Represents a subscription to private message events.
    /// </summary>
    public class PrivateMessageEvent : IScriptEvent
    {
        private readonly Func<PrivateMessageEventArgs, Task> handler;
        private readonly Action<string> logger;
        private EventHandler<MessageEventArgs>? wrappedHandler;

        public PrivateMessageEvent(Func<PrivateMessageEventArgs, Task> handler, Action<string> logger)
        {
            this.handler = handler;
            this.logger = logger;
        }

        public string EventTypeName => "PrivateMessageReceived";

        public void Subscribe(IrcClient client)
        {
            wrappedHandler = async (sender, e) =>
            {
                try
                {
                    var eventArgs = PrivateMessageEventArgs.FromEvent(e.From, e.Message);
                    await handler(eventArgs);
                }
                catch (Exception ex)
                {
                    logger($"Error in PrivateMessage handler: {ex.Message}");
                }
            };

            client.LocalUser.MessageReceived += wrappedHandler;
        }

        public void Unsubscribe(IrcClient client)
        {
            if (wrappedHandler is not null)
            {
                client.LocalUser.MessageReceived -= wrappedHandler;
                wrappedHandler = null;
            }
        }
    }
}

