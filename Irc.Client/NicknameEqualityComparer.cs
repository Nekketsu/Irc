using System.Diagnostics.CodeAnalysis;

namespace Irc.Client
{
    public class NicknameEqualityComparer : EqualityComparer<Nickname>
    {
        private char[] prefixes = { '~', '&', '@', '%', '+' };

        public override bool Equals(Nickname nickname1, Nickname nickname2)
        {
            var x = (string)nickname1;
            var y = (string)nickname2;

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

        public override int GetHashCode([DisallowNull] Nickname nickname)
        {
            var value = (string)nickname;

            if (value is not null && prefixes.Any(value.StartsWith))
            {
                value = value.Substring(1);
            }

            return string.GetHashCode(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public new static NicknameEqualityComparer Default => new();
    }
}
