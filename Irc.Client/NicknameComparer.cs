namespace Irc.Client;

public class NicknameComparer : IComparer<string>
{
    private char[] prefixes = { '~', '&', '@', '%', '+' };

    public int Compare(string x, string y)
    {
        var xIndexOf = Array.IndexOf(prefixes, x[0]);
        var yIndexOf = Array.IndexOf(prefixes, y[0]);

        if (xIndexOf >= 0 && yIndexOf >= 0)
        {
            if (xIndexOf == yIndexOf)
            {
                return string.Compare(x[1..], y[1..], StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                return xIndexOf.CompareTo(yIndexOf);
            }
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

    public static NicknameComparer Default => new();
}
