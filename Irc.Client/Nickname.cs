namespace Irc.Client;

public class Nickname
{
    private static readonly char[] prefixes = { '~', '&', '@', '%', '+' };

    public char? Prefix { get; }
    public string Value { get; }
    public string FullValue { get; }

    private Nickname(string value)
    {
        (FullValue, Value, Prefix) = prefixes.Any(value.StartsWith)
            ? (value, value[1..], value[0])
            : (value, value, (char?)null);
    }

    public static implicit operator Nickname(string nickname)
    {
        return new Nickname(nickname);
    }

    public static explicit operator string(Nickname nickname)
    {
        return nickname.FullValue;
    }

    public override bool Equals(object obj)
    {
        if (obj is Nickname nickname)
        {
            return Value.Equals(nickname.Value, StringComparison.InvariantCultureIgnoreCase);
        }
        else if (obj is string value)
        {
            return Value.Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static bool operator ==(Nickname nickname1, Nickname nickname2)
    {
        return nickname1.Equals(nickname2);
    }

    public static bool operator !=(Nickname nickname1, Nickname nickname2)
    {
        return nickname1.Equals(nickname2);
    }

    public static bool operator ==(string nickname1, Nickname nickname2)
    {
        return nickname2.Equals(nickname1);
    }

    public static bool operator !=(string nickname1, Nickname nickname2)
    {
        return !nickname2.Equals(nickname1);
    }

    public static bool operator ==(Nickname nickname1, string nickname2)
    {
        return nickname1.Equals(nickname2);
    }

    public static bool operator !=(Nickname nickname1, string nickname2)
    {
        return !nickname1.Equals(nickname2);
    }
}
