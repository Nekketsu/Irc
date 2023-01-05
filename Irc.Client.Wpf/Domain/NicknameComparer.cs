using System;
using System.Collections.Generic;

namespace Irc.Client.Wpf.Domain
{
    public class NicknameComparer : IComparer<string>
    {
        private char[] prefixes = { '~', '&', '@', '%', '+' };

        public int Compare(string x, string y)
        {
            var xIndexOf = Array.IndexOf(prefixes, x[0]);
            var yIndexOf = Array.IndexOf(prefixes, y[0]);

            if (xIndexOf >= 0 && yIndexOf >= 0)
            {
                return string.Compare(x.Substring(1), y.Substring(1), StringComparison.InvariantCultureIgnoreCase);
            }
            if (xIndexOf >= 0)
            {
                return -1;
            }
            if (yIndexOf >= 0)
            {
                return 1;
            }

            return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
        }

        public static NicknameComparer Default => new NicknameComparer();
    }
}
