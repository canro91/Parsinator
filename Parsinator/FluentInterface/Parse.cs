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

        public static ParseFromMultiGroupRegex MultiGroupRegex(Regex pattern)
            => new ParseFromMultiGroupRegex(pattern);
    }

    public class KeyBuilder
    {
        public string Key { get; internal set; }

        public ParseFromRegex Regex(Regex regex)
            => new ParseFromRegex(Key, regex);

        public ParseFromValue Value(string value)
            => new ParseFromValue(Key, value);

        public FromLineBuilder FromLine(int lineNumber)
        {
            var builder = new FromLineBuilder
            {
                Key = Key,
                LineNumber = lineNumber
            };
            return builder;
        }

        public FromRegexBuilder FromRegex(Regex regex)
        {
            var builder = new FromRegexBuilder
            {
                Key = Key,
                First = regex
            };
            return builder;
        }

        public FromFirstRegexBuilder FromFirstRegex(Regex regex)
        {
            var builder = new FromFirstRegexBuilder
            {
                Key = Key,
                First = regex
            };
            return builder;
        }

        public ParseFromSplitting SplitBy(string separator)
            => new ParseFromSplitting(Key, separator);
    }

    public class FromLineBuilder
    {
        public string Key { get; internal set; }

        public int LineNumber { get; internal set; }

        public ParseFromLineNumberUntilFirstMatchOfRegex UntilRegex(Regex regex)
            => new ParseFromLineNumberUntilFirstMatchOfRegex(Key, LineNumber, regex);

        public ParseFromLineNumberWithRegex Regex(Regex regex)
            => new ParseFromLineNumberWithRegex(Key, LineNumber, regex);

        public StartingAtBuilder StartingAt(int position)
        {
            var builder = new StartingAtBuilder
            {
                Key = Key,
                LineNumber = LineNumber,
                StartPosition = position
            };
            return builder;
        }
    }

    public class FromRegexBuilder
    {
        public string Key { get; set; }

        public Regex First { get; set; }

        public ParseFromRegexToRegex ToRegex(Regex regex)
            => new ParseFromRegexToRegex(Key, First, regex);

        public IParse ToLastRegex(Regex regex)
            => new ParseFromRegexToLastRegex(Key, First, regex);
    }

    public class FromFirstRegexBuilder
    {
        public string Key { get; internal set; }

        public Regex First { get; internal set; }

        public ParseFromFirstRegexToRegex ToRegex(Regex regex)
            => new ParseFromFirstRegexToRegex(Key, First, regex);

        public IParse ToLastRegex(Regex regex)
            => new ParseFromFirstRegexToLastRegex(Key, First, regex);
    }

    public class StartingAtBuilder
    {
        public string Key { get; internal set; }
        public int LineNumber { get; internal set; }
        public int StartPosition { get; internal set; }

        public IParse Chars(int charCount)
            => new ParseFromLineWithCountAfterPosition(Key, LineNumber, StartPosition, charCount);
    }
}