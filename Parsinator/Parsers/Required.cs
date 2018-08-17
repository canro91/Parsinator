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
            this.PageNumber = p.PageNumber;
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default
        {
            get
            {
                return P.Default ?? throw new ArgumentNullException($"Pattern not found for [{P.Key}]", innerException: null);
            }
        }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var result = P.Parse(line, lineNumber, lineNumberFromBottom);
            if (P.HasMatched)
            {
                this.HasMatched = true;
                return result;
            }
            return new Dictionary<string, string>();
        }
    }
}
