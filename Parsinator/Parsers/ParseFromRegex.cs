using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class ParseFromRegex : ParseWithFactory
    {
        public Regex Pattern { get; private set; }

        public ParseFromRegex(string key, Regex pattern, int? pageNumber, Func<IDictionary<string, string>, string> factory, Func<string> @default)
            : base(key, pageNumber, factory, @default)
        {
            this.Pattern = pattern;
        }

        public ParseFromRegex(string key, Regex pattern, Func<string> @default)
            : this(key, pattern, null, null, @default)
        {
        }

        public ParseFromRegex(string key, Regex pattern, Func<IDictionary<string, string>, string> factory)
            : this(key, pattern, null, factory, null)
        {
        }

        public ParseFromRegex(string key, Regex pattern, Int32 pageNumber)
            : this(key, pattern, pageNumber, null, null)
        {
        }

        public ParseFromRegex(string key, Regex pattern)
            : this(key, pattern, null, null, null)
        {
        }

        public override IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            // TODO Check pattern is not null

            var matches = Pattern.Match(line);
            if (matches.Success)
            {
                HasMatched = true;
                var value = (HasFactory) ? Factory(matches.Enumerate(Pattern)) : matches.Groups[1].Value;
                return new Dictionary<string, string> { { Key, value.Trim() } };
            }
            return new Dictionary<string, string>();
        }
    }
}
