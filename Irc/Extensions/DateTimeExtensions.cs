using System;

namespace Irc.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1);
            var unixTime = (dateTime - epoch).Ticks / TimeSpan.TicksPerSecond;

            return unixTime;
        }
    }
}