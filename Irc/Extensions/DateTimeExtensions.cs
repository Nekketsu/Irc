using System;

namespace Irc.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime dateTime)
        {
            var unixTime = dateTime.Ticks / TimeSpan.TicksPerSecond;

            return unixTime;
        }
    }
}