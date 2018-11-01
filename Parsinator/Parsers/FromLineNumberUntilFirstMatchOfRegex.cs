using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class FromLineNumberUntilFirstMatchOfRegex : ParseWithFactory
    {
        private readonly int LineNumber;
        private readonly Regex Pattern;

        private List<String> _content;
        private Boolean _hasAtLeastOneMatch;

        public FromLineNumberUntilFirstMatchOfRegex(String key, int lineNumber, Regex pattern, Func<IDictionary<String, String>, String> factory)
            : base(key, factory)
        {
            this.LineNumber = lineNumber;
            this.Pattern = pattern;

            _content = new List<string>();
            _hasAtLeastOneMatch = false;
        }

        public FromLineNumberUntilFirstMatchOfRegex(String key, int lineNumber, Regex pattern)
            : this(key: key, lineNumber: lineNumber, pattern: pattern, factory: (allLines) => string.Join(" ", allLines.Values))
        {
        }

        public override IDictionary<String, String> Parse(String line, int lineNumber, int lineNumberFromBottom)
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
