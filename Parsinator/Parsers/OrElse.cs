using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class OrElse : IParse
    {
        private readonly IParse Parser1;
        private readonly IParse Parser2;

        public OrElse(IParse parser1, IParse parser2)
        {
            Parser1 = parser1;
            Parser2 = parser2;
            HasMatched = false;
        }

        public String Key { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var result1 = Parser1.Parse(line, lineNumber, lineNumberFromBottom);
            if (result1.Key != null)
            {
                HasMatched = true;
                return result1;
            }
            else
            {
                var result2 = Parser2.Parse(line, lineNumber, lineNumberFromBottom);
                if (result2.Key != null)
                {
                    HasMatched = true;
                    return result2;
                }
            }
            return new KeyValuePair<string, string>();
        }
    }
}
