using System.Text.RegularExpressions;

namespace Irc.Helpers;

public static class MaskHelper
{
    public static Regex GetRegex(string mask)
    {
        var pattern = GetPattern(mask);
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        return regex;
    }

    private static string GetPattern(string mask)
    {
        var pattern = Regex.Escape(mask);
        pattern = $"^{pattern}$";
        pattern = pattern.Replace("\\*", ".*");
        pattern = pattern.Replace("\\?", ".");

        return pattern;
    }
}