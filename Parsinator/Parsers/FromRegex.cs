using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class FromRegex : IParse
    {
        public String Key { get; private set; }
        public Regex Pattern { get; private set; }
        public Func<GroupCollection, String> Factory { get; set; }
        public Func<String> Default { get; set; }

        public bool HasMatched { get; private set; }

        public FromRegex(String key, Regex pattern, Func<GroupCollection, String> factory, Func<String> @default)
        {
            this.Key = key;
            this.Pattern = pattern;
            this.Factory = factory;
            this.Default = @default;
        }

        public FromRegex(String key, Regex pattern, Func<String> @default)
        {
            this.Key = key;
            this.Pattern = pattern;
            this.Default = @default;
        }

        public FromRegex(String key, Regex pattern, Func<GroupCollection, String> factory)
        {
            this.Key = key;
            this.Pattern = pattern;
            this.Factory = factory;
        }

        public FromRegex(String key, Regex pattern)
        {
            this.Key = key;
            this.Pattern = pattern;
        }

        public KeyValuePair<String, String> Parse(String line, int lineNumber, int lineNumberFromBottom)
        {
            // TODO Check pattern is not null

            var matches = Pattern.Match(line);
            if (matches.Success)
            {
                HasMatched = true;
                var value = (Factory != null) ? Factory(matches.Groups) : matches.Groups[1].Value;
                return new KeyValuePair<string, string>(Key, value.Trim());
            }
            return new KeyValuePair<string, string>();
        }
    }
}
