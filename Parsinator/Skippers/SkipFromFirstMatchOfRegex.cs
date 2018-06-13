using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public class SkipFromFirstMatchOfRegex : ISkip
    {
        private readonly Regex Pattern;

        public SkipFromFirstMatchOfRegex(Regex pattern)
        {
            this.Pattern = pattern;
        }

        public List<List<String>> Skip(List<List<String>> lines)
        {
            var skipped = lines.Select(l => l.TakeWhile(t => !Pattern.IsMatch(t))
                                             .ToList())
                               .ToList();
            return skipped;
        }
    }
}
