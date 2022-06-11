using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class ParseFromLineNumberUntilFirstMatchOfRegex : ParseWithFactory
    {
        private readonly int LineNumber;
        private readonly Regex Pattern;

        private List<string> _content;
        private bool _hasAtLeastOneMatch;

        public ParseFromLineNumberUntilFirstMatchOfRegex(string key, int lineNumber, Regex pattern, Func<IDictionary<string, string>, string> factory)
            : base(key, factory)
        {
            this.LineNumber = lineNumber;
            this.Pattern = pattern;

            _content = new List<string>();
            _hasAtLeastOneMatch = false;
        }

        public ParseFromLineNumberUntilFirstMatchOfRegex(string key, int lineNumber, Regex pattern)
            : this(key: key, lineNumber: lineNumber, pattern: pattern, factory: (allLines) => string.Join(" ", allLines.Values))
        {
        }

        public override IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
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
                    var value = Factory(_content.Enumerate());
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
