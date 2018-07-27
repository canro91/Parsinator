using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class Validate : IParse
    {
        private readonly Predicate<string> Predicate;
        private readonly IParse P;

        public Validate(Predicate<string> predicate, IParse p)
        {
            this.Predicate = predicate;
            this.P = p;
            this.Key = p.Key;
            this.PageNumber = p.PageNumber;
            this.Default = p.Default;
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var result = P.Parse(line, lineNumber, lineNumberFromBottom);
            if (P.HasMatched)
            {
                this.HasMatched = true;
                if (!Predicate(result.Value))
                    throw new ArgumentException($"Invalid parsed value for [{P.Key}]. Value: [{result.Value}]");

                return result;
            }
            return new KeyValuePair<string, string>();
        }
    }
}
