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
            this.Pattern = pattern;
        }

        public List<List<string>> Skip(List<List<string>> lines)
        {
            var skipped = lines.Select(l => l.Where(t => Pattern.IsMatch(t)).ToList())
                               .ToList();
            return skipped;
        }
    }
}
