using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class Not : IParse
    {
        private readonly IParse P;

        public Not(IParse p)
        {
            this.P = p;
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default
        {
            get
            {
                var @default = P.Default?.Invoke();
                if (!string.IsNullOrEmpty(@default))
                    throw new ArgumentException($"Pattern for [{P.Key}] expected to fail. Value: [{P.Key}:{@default}]", innerException: null);

                return P.Default;
            }
        }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var result = P.Parse(line, lineNumber, lineNumberFromBottom);
            if (P.HasMatched)
            {
                throw new ArgumentException($"Pattern for [{P.Key}] expected to fail. Value: [{string.Join(",", result.Select(kv => kv.Key + ":" + kv.Value).ToArray())}]", innerException: null);
            }
            return new Dictionary<string, string>();
        }
    }
}
