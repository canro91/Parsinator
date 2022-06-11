using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class SkipFromFirstRegexToLastRegex : ISkip
    {
        private readonly Regex First;
        private readonly Regex Last;

        public SkipFromFirstRegexToLastRegex(Regex first, Regex last)
        {
            First = first;
            Last = last;
        }

        public IEnumerable<IEnumerable<string>> Skip(IEnumerable<IEnumerable<string>> lines)
        {
            var skipped = lines.Select(l => l.TakeWhile(t => !First.IsMatch(t))
                                             .Union(l.SkipWhile(t => !Last.IsMatch(t))
                                                     // To skip the regex match itself
                                                     .Skip(1))
                                            .ToList())
                                .ToList();
            return skipped;
        }
    }
}
