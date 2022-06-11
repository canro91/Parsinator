using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class ParseFromSplitting : IParse
    {
        private readonly string Separator;
        private readonly int? SkipFirsts;

        public ParseFromSplitting(string key, string separator)
            : this(key, separator, null)
        {
        }

        public ParseFromSplitting(string key, string separator, int? skipFirsts)
        {
            this.Key = key;
            this.Separator = separator;
            this.SkipFirsts = skipFirsts;
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var splitted = line.Split(new string[] { Separator }, StringSplitOptions.None)
                               .Select(t => t.Trim());
            var matched = SkipFirsts.HasValue ? splitted.Skip(SkipFirsts.Value) : splitted;

            this.HasMatched = true;
            return matched.Enumerate(prefix: this.Key);
        }
    }
}
