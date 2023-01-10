using Irc.Messages;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Irc.Client.Wpf.MessageHandlers
{
    public interface IMessageHandler<T> where T : Message
    {
        Task HandleAsync(T message);
    }

    public interface IMessageHandler
    {
        public static async Task<bool> HandleAsync(Message message)
        {
            if (message is null)
            {
                return false;
            }

            var messageHandlerType = Assembly.GetExecutingAssembly().ExportedTypes.SingleOrDefault(type =>
                type.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IMessageHandler<>) &&
                    i.GetGenericArguments().Any(a => a == message.GetType())));

            if (messageHandlerType is null)
            {
                return false;
            }

            var messageHandler = ActivatorUtilities.CreateInstance(App.Current.Services, messageHandlerType);
            var handleAsyncMethod = messageHandlerType.GetMethod(nameof(IMessageHandler<Message>.HandleAsync));

            var taskResult = (Task)handleAsyncMethod.Invoke(messageHandler, new object[] { message });

            await taskResult;

            return true;
        }
    }
}
