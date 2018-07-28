using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class SkipLineCountFromEnd : ISkip
    {
        private readonly int LineCount;

        public SkipLineCountFromEnd(int lineCount = 1)
        {
            this.LineCount = lineCount;
        }

        public List<List<string>> Skip(List<List<string>> lines)
        {
            var skipped = lines.Select(l => l.Take(l.Count - LineCount).ToList())
                               .ToList();
            return skipped;
        }
    }
}
