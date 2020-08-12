﻿using System.Text.RegularExpressions;

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
    }

    public class FromLineBuilder
    {
        public string Key { get; internal set; }

        public int LineNumber { get; internal set; }

        public ParseFromLineNumberUntilFirstMatchOfRegex UntilRegex(Regex regex)
            => new ParseFromLineNumberUntilFirstMatchOfRegex(Key, LineNumber, regex);

        public ParseFromLineNumberWithRegex Regex(Regex regex)
            => new ParseFromLineNumberWithRegex(Key, LineNumber, regex);
    }

    public class FromRegexBuilder
    {
        public string Key { get; set; }

        public Regex First { get; set; }

        public ParseFromRegexToRegex ToRegex(Regex regex)
            => new ParseFromRegexToRegex(Key, First, regex);
    }
}