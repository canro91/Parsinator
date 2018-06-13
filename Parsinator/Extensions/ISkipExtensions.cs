using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public static class ISkipExtensions
    {
        public static List<List<String>> Chain(this IList<ISkip> skips, List<List<String>> lines)
        {
            List<List<String>> pages = lines;
            if (skips != null && skips.Any())
            {
                foreach (var s in skips)
                {
                    pages = s.Skip(pages);
                }
            }
            return pages;
        }
    }
}
