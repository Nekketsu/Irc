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
                    var prefix = command;
                    command = messageSplit[1];
                    parameters[0] = prefix;
                }

                var messageType = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .SingleOrDefault(type => type.IsSubclassOf(typeof(Message))
                                          && string.Equals(type.GetCustomAttribute<CommandAttribute>()?.Command, command, StringComparison.InvariantCultureIgnoreCase));

                if (messageType is not null)
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

        public string Command => this.GetType().GetCustomAttribute<CommandAttribute>().Command;
    }
}