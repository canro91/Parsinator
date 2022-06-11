using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class ParseFromRegexToLastRegex : ParseWithFactory
    {
        private readonly Regex FirstPattern;
        private readonly Regex SecondPattern;

        private List<string> _content;
        private bool _hasAtLeastOneMatch;

        public ParseFromRegexToLastRegex(string key, Regex first, Regex second, Func<IDictionary<string, string>, string> factory, Func<string> @default)
            : base(key, factory)
        {
            this.FirstPattern = first;
            this.SecondPattern = second;

            this._content = new List<string>();
            this._hasAtLeastOneMatch = false;
        }

        public ParseFromRegexToLastRegex(string key, Regex first, Regex second)
            : this(key, first, second, factory: (allLines) => string.Join(" ", allLines.Values), @default: null)
        {
        }

        public override IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
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

                    _content.Add(line.Trim());

                    var value = Factory(_content.Enumerate()) ?? Default();
                    return new Dictionary<string, string> { { Key, value } };
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
