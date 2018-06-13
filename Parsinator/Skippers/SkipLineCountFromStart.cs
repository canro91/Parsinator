using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class SkipLineCountFromStart : ISkip
    {
        private readonly int LineCount;

        public SkipLineCountFromStart(int lineCount = 1)
        {
            this.LineCount = lineCount;
        }

        public List<List<string>> Skip(List<List<string>> lines)
        {
            var skipped = lines.Select(l => l.Skip(LineCount).ToList())
                               .ToList();
            return skipped;
        }
    }
}
