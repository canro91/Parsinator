using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public static class ISkipExtensions
    {
        internal static IEnumerable<IEnumerable<string>> Chain(this IEnumerable<ISkip> skips, IEnumerable<IEnumerable<string>> lines)
        {
            return skips != null && skips.Any()
                ? skips.Aggregate(lines, (pages, skip) => skip.Skip(pages))
                : lines;
        }
    }
}
