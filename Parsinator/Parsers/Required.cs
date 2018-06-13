using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class Required : IParse
    {
        private readonly IParse P;

        public Required(IParse p)
        {
            this.P = p;
            this.Key = p.Key;
            this.Default = p.Default;
        }

        public String Key { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var result = P.Parse(line, lineNumber, lineNumberFromBottom);
            if (P.HasMatched)
            {
                this.HasMatched = true;
                return result;
            }
            else if (P.Default != null)
            {
                this.Default = P.Default;
                return new KeyValuePair<string, string>();
            }
            else
            {
                throw new ArgumentNullException($"Pattern not found for [{P.Key}] in [{line}]", innerException: null);
            }
        }
    }
}
