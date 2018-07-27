using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class FromLineNumberWithRegex : IParse
    {
        private readonly int LineNumber;
        private readonly Regex Pattern;
        private readonly Func<GroupCollection, String> Factory;

        public FromLineNumberWithRegex(String key, int lineNumber, Regex pattern, Func<GroupCollection, String> factory)
        {
            this.Key = key;
            this.LineNumber = lineNumber;
            this.Pattern = pattern;
            this.Factory = factory;
        }

        public FromLineNumberWithRegex(string key, int lineNumber, Regex pattern)
        {
            this.Key = key;
            this.LineNumber = lineNumber;
            this.Pattern = pattern;
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<String, String> Parse(String line, int lineNumber, int lineNumberFromBottom)
        {
            if (lineNumber == this.LineNumber || (this.LineNumber < 0 && lineNumberFromBottom == this.LineNumber))
            {
                var matches = Pattern.Match(line);
                if (matches.Success)
                {
                    HasMatched = true;
                    var value = (Factory != null) ? Factory(matches.Groups) : matches.Groups[1].Value;
                    return new KeyValuePair<string, string>(Key, value.Trim());
                }
            }
            return new KeyValuePair<string, string>();
        }
    }
}
