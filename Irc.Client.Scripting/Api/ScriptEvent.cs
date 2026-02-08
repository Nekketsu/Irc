namespace Irc.Client.Scripting.Api
{
    /// <summary>
    /// Interface for all script events that can be subscribed to.
    /// </summary>
    public interface IScriptEvent
    {
        /// <summary>
        /// Subscribes this event handler to the IRC client events.
        /// </summary>
        void Subscribe(IrcClient client);

        /// <summary>
        /// Unsubscribes this event handler from the IRC client events.
        /// </summary>
        void Unsubscribe(IrcClient client);

        /// <summary>
        /// Gets the display name of this event type for UI purposes.
        /// </summary>
        string EventTypeName { get; }
    }
}
