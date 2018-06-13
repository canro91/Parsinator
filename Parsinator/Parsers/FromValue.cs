using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class FromValue : IParse
    {
        public FromValue(string key, string value)
        {
            this.Key = key;
            this.Default = () => value;
        }

        public string Key { get; private set; }

        public bool HasMatched { get; private set; }

        public Func<string> Default { get; private set; }

        public KeyValuePair<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            HasMatched = true;
            return new KeyValuePair<string, string>(Key, Default());
        }
    }
}
