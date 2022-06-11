using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class SkipIfMatches : ISkip
    {
        private readonly Regex Pattern;

        public SkipIfMatches(Regex pattern)
        {
            Pattern = pattern;
        }

        public IEnumerable<IEnumerable<string>> Skip(IEnumerable<IEnumerable<string>> lines)
        {
            var skipped = lines.Select(l => l.Where(t => !Pattern.IsMatch(t)).ToList())
                               .ToList();
            return skipped;
        }
    }
}
