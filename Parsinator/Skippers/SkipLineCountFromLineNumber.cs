using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class SkipLineCountFromLineNumber : ISkip
    {
        private readonly int LineNumber;
        private readonly int LineCount;

        public SkipLineCountFromLineNumber(int lineNumber, int lineCount)
        {
            LineNumber = lineNumber;
            LineCount = lineCount;
        }

        public IEnumerable<IEnumerable<string>> Skip(IEnumerable<IEnumerable<string>> lines)
        {
            var skipped = lines.Select(l => l.Take(LineNumber - 1)
                                             .Skip(LineCount)
                                             .ToList())
                               .ToList();
            return skipped;
        }
    }
}
