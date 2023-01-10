using Irc.Messages;
using System.Reflection;

namespace Irc.Client.MessageHandlers
{
    internal interface IMessageHandler<T> where T : Message
    {
        void Handle(T message, IrcClient ircClient);
    }

    internal interface IMessageHandler
    {
        public static void Handle(Message message, IrcClient ircClient)
        {
            if (message is null)
            {
                return;
            }

            var messageHandlerType = Assembly.GetExecutingAssembly().DefinedTypes.SingleOrDefault(type =>
               type.GetInterfaces().Any(i =>
                   i.IsGenericType &&
                   i.GetGenericTypeDefinition() == typeof(IMessageHandler<>) &&
                   i.GetGenericArguments().Any(a => a == message.GetType())));

            if (messageHandlerType is null)
            {
                return;
            }

            var messageHandler = Activator.CreateInstance(messageHandlerType);
            var handleMethod = messageHandlerType.GetMethod(nameof(IMessageHandler<Message>.Handle));

            handleMethod.Invoke(messageHandler, new object[] { message, ircClient });
        }
    }
}
