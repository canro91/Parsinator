using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class ParseFromLineNumberUntilFirstMatchOfRegex : IParse
    {
        private readonly int LineNumber;
        private readonly Regex Pattern;

        private List<string> _content;
        private bool _hasAtLeastOneMatch;

        public ParseFromLineNumberUntilFirstMatchOfRegex(string key, int lineNumber, Regex pattern)
        {
            this.Key = key;
            this.LineNumber = lineNumber;
            this.Pattern = pattern;

            _content = new List<string>();
            _hasAtLeastOneMatch = false;
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            if (lineNumber == this.LineNumber || (this.LineNumber < 0 && lineNumberFromBottom == this.LineNumber))
            {
                _content.Add(line.Trim());
                _hasAtLeastOneMatch = true;
            }
            else if (_hasAtLeastOneMatch)
            {
                var matches = Pattern.Match(line);
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
