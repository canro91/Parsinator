using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class SkipBlankLines : ISkip
    {
        public IEnumerable<IEnumerable<string>> Skip(IEnumerable<IEnumerable<string>> lines)
        {
            var skipped = lines.Select(l => l.Where(t => !string.IsNullOrEmpty(t))
                                             .ToList())
                               .ToList();
            return skipped;
        }
    }
}
