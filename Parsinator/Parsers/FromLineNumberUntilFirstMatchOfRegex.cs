using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class FromLineNumberUntilFirstMatchOfRegex : IParse
    {
        private readonly int LineNumber;
        private readonly Regex Pattern;
        private readonly Func<List<String>, String> Factory;

        private List<String> _content;
        private Boolean _hasAtLeastOneMatch;

        public FromLineNumberUntilFirstMatchOfRegex(String key, int lineNumber, Regex pattern, Func<List<String>, String> factory)
        {
            this.Key = key;
            this.LineNumber = lineNumber;
            this.Pattern = pattern;
            this.Factory = factory;

            _content = new List<string>();
            _hasAtLeastOneMatch = false;
        }

        public FromLineNumberUntilFirstMatchOfRegex(String key, int lineNumber, Regex pattern)
            : this(key: key, lineNumber: lineNumber, pattern: pattern, factory: (allLines) => string.Join(" ", allLines))
        {
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<String, String> Parse(String line, int lineNumber, int lineNumberFromBottom)
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
                    var value = Factory(_content);
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
