namespace Irc.Client
{
    public class Nickname
    {
        private static readonly char[] prefixes = { '~', '&', '@', '%', '+' };

        private readonly string value;

        private Nickname(string value)
        {
            this.value = value;
        }

        public static implicit operator Nickname(string nickname)
        {
            return new Nickname(nickname);
        }

        public static explicit operator string(Nickname nickname)
        {
            return nickname.value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Nickname nickname)
            {
                var nicknameValue = nickname.value;
                if (prefixes.Any(nicknameValue.StartsWith))
                {
                    nicknameValue = nicknameValue.Substring(1);
                }
                return value.Equals(nickname.value, StringComparison.InvariantCultureIgnoreCase);
            }
            else if (obj is string value)
            {
                if (prefixes.Any(value.StartsWith))
                {
                    value = value.Substring(1);
                }
                return this.value.Equals(value, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value;
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
}
