using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class SkipLineCountFromStart : ISkip
    {
        private readonly int LineCount;

        public SkipLineCountFromStart(int lineCount = 1)
        {
            LineCount = lineCount;
        }

        public IEnumerable<IEnumerable<string>> Skip(IEnumerable<IEnumerable<string>> lines)
        {
            var skipped = lines.Select(l => l.Skip(LineCount).ToList())
                               .ToList();
            return skipped;
        }
    }
}
