using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class FromLineNumberWithRegex : ParseWithFactory
    {
        private readonly int LineNumber;
        private readonly Regex Pattern;

        public FromLineNumberWithRegex(String key, int lineNumber, Regex pattern, Func<IDictionary<String, String>, String> factory)
            : base(key, factory)
        {
            this.LineNumber = lineNumber;
            this.Pattern = pattern;
        }

        public FromLineNumberWithRegex(string key, int lineNumber, Regex pattern)
            : this(key, lineNumber, pattern, null)
        {
        }

        public override IDictionary<String, String> Parse(String line, int lineNumber, int lineNumberFromBottom)
        {
            if (lineNumber == this.LineNumber || (this.LineNumber < 0 && lineNumberFromBottom == this.LineNumber))
            {
                var matches = Pattern.Match(line);
                if (matches.Success)
                {
                    HasMatched = true;
                    var value = (HasFactory) ? Factory(matches.Enumerate(Pattern)) : matches.Groups[1].Value;
                    return new Dictionary<string, string> { { Key, value.Trim() } };
                }
            }
            return new Dictionary<string, string>();
        }
    }
}
