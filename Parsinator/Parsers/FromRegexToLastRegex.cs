using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class FromRegexToLastRegex : ParseWithFactory
    {
        private readonly Regex FirstPattern;
        private readonly Regex SecondPattern;

        private List<String> _content;
        private Boolean _hasAtLeastOneMatch;

        public FromRegexToLastRegex(String key, Regex first, Regex second, Func<IDictionary<String, String>, String> factory, Func<String> @default)
            : base(key, factory)
        {
            this.FirstPattern = first;
            this.SecondPattern = second;

            this._content = new List<string>();
            this._hasAtLeastOneMatch = false;
        }

        public FromRegexToLastRegex(String key, Regex first, Regex second)
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
