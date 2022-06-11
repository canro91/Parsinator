using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Parsinator
{
    public class SkipBeforeRegexAndAfterRegex : ISkip
    {
        private readonly Regex Before;
        private readonly Regex After;

        public SkipBeforeRegexAndAfterRegex(Regex before, Regex after)
        {
            Before = before;
            After = after;
        }

        public IEnumerable<IEnumerable<string>> Skip(IEnumerable<IEnumerable<string>> lines)
        {
            var skipped = lines.Select(l => l.SkipWhile(t => !Before.IsMatch(t))
                                             // To skip the regex match itself
                                             .Skip(1)
                                             .TakeWhile(t => !After.IsMatch(t))
                                             .ToList())
                               .ToList();
            return skipped;
        }
    }
}
