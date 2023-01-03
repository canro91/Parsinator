using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class ParseFromLineNumberWithRegex : IParse
    {
        private readonly int LineNumber;
        private readonly Regex Pattern;

        public ParseFromLineNumberWithRegex(string key, int lineNumber, Regex pattern)
        {
            this.Key = key;
            this.LineNumber = lineNumber;
            this.Pattern = pattern;
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            if (lineNumber == this.LineNumber || (this.LineNumber < 0 && lineNumberFromBottom == this.LineNumber))
            {
                var matches = Pattern.Match(line);
                if (matches.Success)
                {
                    HasMatched = true;
                    var value = matches.Groups[1].Value;
                    return new Dictionary<string, string> { { Key, value.Trim() } };
                }
            }
            return new Dictionary<string, string>();
        }
    }
}
