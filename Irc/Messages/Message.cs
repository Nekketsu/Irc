using System.Diagnostics;
using System.Reflection;

namespace Irc.Messages;

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
                try
                {
                    var parseMethod = messageType.GetMethod(nameof(Parse), BindingFlags.Public | BindingFlags.Static);

                    var messageInstance = (parseMethod is not null)
                        ? (Message)parseMethod.Invoke(null, new[] { message })
                        : (Message)Activator.CreateInstance(messageType, parameters);

                    return messageInstance;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Parsing exception: {ex.Message}");
                }
            }
        }

        return null;
    }

    public string Command => GetType().GetCustomAttribute<CommandAttribute>().Command;
}