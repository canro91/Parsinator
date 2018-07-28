using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class OrElse : IParse
    {
        private readonly IParse Parser1;
        private readonly IParse Parser2;

        // Since parsers are applied in pages in order, first parser
        // should have a lower page number than second parser. Otherwise
        // second parser won't be ever evaluated.
        public OrElse(IParse parser1, IParse parser2)
        {
            Parser1 = parser1;
            Parser2 = parser2;
            PageNumber = parser1.PageNumber;
            HasMatched = false;
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
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
                PageNumber = Parser2.PageNumber;
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
