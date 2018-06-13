using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class SkipBlankLines : ISkip
    {
        public List<List<string>> Skip(List<List<string>> lines)
        {
            var skipped = lines.Select(l => l.Where(t => !string.IsNullOrEmpty(t))
                                             .ToList())
                               .ToList();
            return skipped;
        }
    }
}
