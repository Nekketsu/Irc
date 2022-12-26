﻿using Microsoft.Extensions.Logging;

namespace Irc.Server.Console
{
    public class ConsoleLogger : ILogger
    {
        private void WriteLine(string message)
        {
            System.Console.WriteLine(message);
        }

        private void WriteColoredLine(string message, ConsoleColor color)
        {
            var foregroundColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.Error.WriteLine(message);
            System.Console.ForegroundColor = foregroundColor;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Information:
                    WriteLine(state.ToString());
                    break;
                case LogLevel.Debug:
                    WriteColoredLine(state.ToString(), ConsoleColor.DarkGreen);
                    break;
                case LogLevel.Warning:
                    WriteColoredLine(state.ToString(), ConsoleColor.DarkYellow);
                    break;
                case LogLevel.Error:
                    WriteColoredLine(state.ToString(), ConsoleColor.DarkRed);
                    break;
                default:
                    WriteColoredLine(state.ToString(), ConsoleColor.DarkBlue);
                    break;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var enabledLogLevels = new[] { LogLevel.Information, LogLevel.Debug, LogLevel.Warning, LogLevel.Error };

            return enabledLogLevels.Contains(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
