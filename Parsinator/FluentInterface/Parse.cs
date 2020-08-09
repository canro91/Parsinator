using System.Text.RegularExpressions;

namespace Parsinator.FluentInterface
{
    public class Parse
    {
        public static KeyBuilder Key(string key)
        {
            var builder = new KeyBuilder
            {
                Key = key
            };
            return builder;
        }
    }

    public class KeyBuilder
    {
        public string Key { get; internal set; }

        public ParseFromRegex Regex(Regex regex)
            => new ParseFromRegex(Key, regex);

        public IParse Value(string value)
            => new ParseFromValue(Key, value);
    }
}