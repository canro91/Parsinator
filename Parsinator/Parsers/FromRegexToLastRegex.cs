using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class FromRegexToLastRegex : IParse
    {
        private readonly Regex FirstPattern;
        private readonly Regex SecondPattern;
        private readonly Func<List<String>, String> Factory;

        private List<String> _content;
        private Boolean _hasAtLeastOneMatch;

        public FromRegexToLastRegex(String key, Regex first, Regex second, Func<List<String>, String> factory, Func<String> @default)
        {
            this.Key = key;
            this.FirstPattern = first;
            this.SecondPattern = second;
            this.Factory = factory;
            this.Default = @default;

            this._content = new List<string>();
            this._hasAtLeastOneMatch = false;
        }

        public FromRegexToLastRegex(String key, Regex first, Regex second)
            : this(key, first, second, factory: (allLines) => string.Join(" ", allLines), @default: null)
        {
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
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

                    var value = Factory(_content) ?? Default();
                    return new KeyValuePair<string, string>(Key, value);
                }
                else
                {
                    _content.Add(line.Trim());
                }
            }

            return new KeyValuePair<string, string>();
        }
    }
}
