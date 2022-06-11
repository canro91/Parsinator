using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public static class ISkipExtensions
    {
        internal static IEnumerable<IEnumerable<string>> Chain(this IEnumerable<ISkip> skips, IEnumerable<IEnumerable<string>> lines)
        {
            var pages = lines;
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
