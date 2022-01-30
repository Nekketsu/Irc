using System;
using System.Linq;
using System.Reflection;

namespace Irc.Messages
{
    public abstract class Message : IMessage
    {
        public static Message Parse(string message)
        {
            var messageSplit = message.Split();
            if (messageSplit.Any())
            {
                var command = messageSplit.First();
                var parameters = messageSplit.Skip(1).ToArray();

                if (command.StartsWith(":") && messageSplit.Length >= 2)
                {
                    command = messageSplit[1];
                    parameters = parameters.Skip(1).Prepend(messageSplit[0]).ToArray();
                }

                var messageType = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .SingleOrDefault(type => type.IsSubclassOf(typeof(Message)) && IsMessage(type.Name, command));

                if (messageType != null)
                {
                    Message messageInstance;
                    try
                    {
                        var parseMethod = messageType.GetMethod(nameof(Parse), BindingFlags.Public | BindingFlags.Static);

                        messageInstance = (parseMethod != null)
                            ? (Message)parseMethod.Invoke(null, new[] { message })
                            : (Message)Activator.CreateInstance(messageType, parameters);

                        return messageInstance;
                    }
                    catch { };
                }
            }

            return null;
        }

        private static bool IsMessage(string typeName, string commandName)
        {
            var command = typeName.Substring(0, typeName.Length - nameof(Message).Length);

            return string.Equals(command, commandName, StringComparison.OrdinalIgnoreCase);
        }

        public string Command { get; private set; }

        public Message()
        {
            var name = GetType().Name;
            if (!name.EndsWith(nameof(Message)))
            {
                throw new ArgumentException($"The class \"{name}\" doesn't follow convention of ending with \"{nameof(Message)}\"");
            }

            Command = name.Substring(0, name.Length - nameof(Message).Length).ToUpper();
        }
    }
}