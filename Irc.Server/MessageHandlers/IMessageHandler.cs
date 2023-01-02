using Irc.Messages;
using System.Reflection;

namespace Irc.Server.MessageHandlers
{
    public class MessageHandler<T> where T : Message
    {
        public virtual Task<bool> HandleAsync(T message, IrcClient ircClient)
        {
            return Task.FromResult(true);
        }
    }

    public static class MessageHandler
    {
        public static async Task<bool> HandleAsync(Message message, IrcClient ircClient)
        {
            if (message is null)
            {
                return true;
            }

            var messageHandlerType = Assembly.GetExecutingAssembly().ExportedTypes.SingleOrDefault(type =>
                type.BaseType.IsGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(MessageHandler<>) &&
                type.BaseType.GetGenericArguments().Any(a => a == message.GetType()));

            var messageHandler = Activator.CreateInstance(messageHandlerType);
            var handleAsyncMethod = messageHandlerType.GetMethod(nameof(MessageHandler<Message>.HandleAsync));

            var taskResult = (Task<bool>)handleAsyncMethod.Invoke(messageHandler, new object[] { message, ircClient });

            return await taskResult;
        }
    }
}
