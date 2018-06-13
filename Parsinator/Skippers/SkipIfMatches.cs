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
            this.Pattern = pattern;
        }

        public List<List<string>> Skip(List<List<string>> lines)
        {
            var skipped = lines.Select(l => l.Where(t => !Pattern.IsMatch(t)).ToList())
                               .ToList();
            return skipped;
        }
    }
}
