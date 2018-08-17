using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class FromRegex : IParse
    {
        public Regex Pattern { get; private set; }
        public Func<GroupCollection, String> Factory { get; set; }

        public FromRegex(String key, Regex pattern, Int32? pageNumber, Func<GroupCollection, String> factory, Func<String> @default)
        {
            this.Key = key;
            this.PageNumber = pageNumber;
            this.Pattern = pattern;
            this.Factory = factory;
            this.Default = @default;
        }

        public FromRegex(String key, Regex pattern, Func<String> @default)
            : this(key, pattern, null, null, @default)
        {
        }

        public FromRegex(String key, Regex pattern, Func<GroupCollection, String> factory)
            : this(key, pattern, null, factory, null)
        {
        }

        public FromRegex(String key, Regex pattern, Int32 pageNumber)
            : this(key, pattern, pageNumber, null, null)
        {
        }

        public FromRegex(String key, Regex pattern)
            : this(key, pattern, null, null, null)
        {
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<String, String> Parse(String line, int lineNumber, int lineNumberFromBottom)
        {
            // TODO Check pattern is not null

            var matches = Pattern.Match(line);
            if (matches.Success)
            {
                HasMatched = true;
                var value = (Factory != null) ? Factory(matches.Groups) : matches.Groups[1].Value;
                return new Dictionary<string, string> { { Key, value.Trim() } };
            }
            return new Dictionary<string, string>();
        }
    }
}
