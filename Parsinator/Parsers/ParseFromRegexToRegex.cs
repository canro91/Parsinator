using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class ParseFromRegexToRegex : IParse
    {
        private readonly Regex FirstPattern;
        private readonly Regex SecondPattern;

        private List<string> _content;
        private bool _hasAtLeastOneMatch;

        public ParseFromRegexToRegex(string key, Regex first, Regex second, Func<string> @default)
        {
            this.Key = key;
            this.Default = @default;
            this.FirstPattern = first;
            this.SecondPattern = second;

            this._content = new List<string>();
            this._hasAtLeastOneMatch = false;
        }

        public ParseFromRegexToRegex(string key, Regex first, Regex second)
            : this(key: key, first: first, second: second, @default: null)
        {
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            // TODO Check pattern is not null

            if (!_hasAtLeastOneMatch && FirstPattern.IsMatch(line))
            {
                _hasAtLeastOneMatch = true;
            }
            else if (_hasAtLeastOneMatch)
            {
                var matches = SecondPattern.Match(line);
                if (matches.Success)
                {
                    HasMatched = true;
                    return _content.Enumerate(prefix: Key);
                }
                else
                {
                    _content.Add(line.Trim());
                }
            }

            return new Dictionary<string, string>();
        }
    }
}
