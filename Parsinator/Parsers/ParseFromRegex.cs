using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class ParseFromRegex : IParse
    {
        private readonly Regex Pattern;
        private readonly Func<IDictionary<string, string>, string> Factory;

        public ParseFromRegex(string key, Regex pattern, int? pageNumber, Func<IDictionary<string, string>, string> factory, Func<string> @default)
        {
            this.Key = key;
            this.PageNumber = pageNumber;
            this.Default = @default;

            this.Pattern = pattern;
            this.Factory = factory;
        }

        public ParseFromRegex(string key, Regex pattern, Func<string> @default)
            : this(key, pattern, null, null, @default)
        {
        }

        public ParseFromRegex(string key, Regex pattern, Func<IDictionary<string, string>, string> factory)
            : this(key, pattern, null, factory, null)
        {
        }

        public ParseFromRegex(string key, Regex pattern, int pageNumber)
            : this(key, pattern, pageNumber, null, null)
        {
        }

        public ParseFromRegex(string key, Regex pattern)
            : this(key, pattern, null, null, null)
        {
        }

        public string Key { get; private set; }
        public int? PageNumber { get; protected set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; protected set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            // TODO Check pattern is not null

            var matches = Pattern.Match(line);
            if (matches.Success)
            {
                HasMatched = true;
                var value = Factory != null
                    ? Factory(matches.Enumerate(Pattern))
                    : matches.Groups[1].Value;
                return new Dictionary<string, string> { { Key, value.Trim() } };
            }

            return new Dictionary<string, string>();
        }
    }
}
