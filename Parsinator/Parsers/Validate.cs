using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class Validate : IParse
    {
        private readonly Predicate<IDictionary<string, string>> Predicate;
        private readonly IParse P;

        public Validate(Predicate<IDictionary<string, string>> predicate, IParse p)
        {
            this.Predicate = predicate;
            this.P = p;
            this.Key = p.Key;
            this.PageNumber = p.PageNumber;
            this.Default = p.Default;
        }

        public string Key { get; private set; }
        public int? PageNumber { get; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var result = P.Parse(line, lineNumber, lineNumberFromBottom);
            if (P.HasMatched)
            {
                this.HasMatched = true;
                if (!Predicate(result))
                    throw new ArgumentException($"Invalid parsed value for [{P.Key}]. Value: [{string.Join(",", result.Select(kv => kv.Key + ":" + kv.Value).ToArray())}]");

                return result;
            }
            return new Dictionary<string, string>();
        }
    }
}
