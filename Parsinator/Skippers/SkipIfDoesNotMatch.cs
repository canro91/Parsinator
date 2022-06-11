using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class SkipIfDoesNotMatch : ISkip
    {
        private readonly Regex Pattern;

        public SkipIfDoesNotMatch(Regex pattern)
        {
            Pattern = pattern;
        }

        public IEnumerable<IEnumerable<string>> Skip(IEnumerable<IEnumerable<string>> lines)
        {
            var skipped = lines.Select(l => l.Where(t => Pattern.IsMatch(t)).ToList())
                               .ToList();
            return skipped;
        }
    }
}
