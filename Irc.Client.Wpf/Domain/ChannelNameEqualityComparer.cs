using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Irc.Client.Wpf.Domain
{
    public class ChannelNameEqualityComparer : EqualityComparer<string>
    {
        private char[] prefixes = { '#', '&' };

        public override bool Equals(string x, string y)
        {
            if (x is not null && prefixes.Any(x.StartsWith))
            {
                x = x.Substring(1);
            }
            if (y is not null && prefixes.Any(y.StartsWith))
            {
                y = y.Substring(1);
            }

            return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode([DisallowNull] string obj)
        {
            if (obj is not null && prefixes.Any(obj.StartsWith))
            {
                obj = obj.Substring(1);
            }

            return string.GetHashCode(obj, StringComparison.InvariantCultureIgnoreCase);
        }

        public new static ChannelNameEqualityComparer Default => new();
    }
}
