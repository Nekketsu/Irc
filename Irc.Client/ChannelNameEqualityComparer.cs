using System.Diagnostics.CodeAnalysis;

namespace Irc.Client;

public class ChannelNameEqualityComparer : EqualityComparer<string>
{
    private char[] prefixes = { '#', '&' };

    public override bool Equals(string x, string y)
    {
        if (x is not null && prefixes.Any(x.StartsWith))
        {
            x = x[1..];
        }
        if (y is not null && prefixes.Any(y.StartsWith))
        {
            y = y[1..];
        }

        return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
    }

    public override int GetHashCode([DisallowNull] string obj)
    {
        if (obj is not null && prefixes.Any(obj.StartsWith))
        {
            obj = obj[1..];
        }

        return string.GetHashCode(obj, StringComparison.InvariantCultureIgnoreCase);
    }

    public new static ChannelNameEqualityComparer Default => new();
}
