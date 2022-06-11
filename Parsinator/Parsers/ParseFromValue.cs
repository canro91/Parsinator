using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class ParseFromValue : IParse
    {
        public ParseFromValue(string key, string value)
        {
            this.Key = key;
            this.Default = () => value;
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            HasMatched = true;
            return new Dictionary<string, string> { { Key, Default() } };
        }
    }
}
