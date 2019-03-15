using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class ParseFromMultiGroupRegex : IParse
    {
        private readonly Regex Pattern;

        public ParseFromMultiGroupRegex(Regex pattern)
        {
            this.Key = "";
            this.Pattern = pattern;
        }
        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            // TODO Check pattern is not null

            var matches = Pattern.Match(line);
            if (matches.Success)
            {
                HasMatched = true;
                return matches.Enumerate(Pattern);
            }
            return new Dictionary<string, string>();
        }
    }
}
