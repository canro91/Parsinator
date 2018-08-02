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

        public List<List<string>> Skip(List<List<string>> lines)
        {
            var skipped = lines.Select(l => l.Take(LineNumber - 1)
                                             .Skip(LineCount)
                                             .ToList())
                               .ToList();
            return skipped;
        }
    }
}
