using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public static class IEnumerableExtensions
    {
        internal static IDictionary<string, string> Enumerate(this IEnumerable<string> self)
        {
            return self.Select((item, i) => new { Item = item, Index = i })
                       .ToDictionary(k => k.Index.ToString(), v => v.Item);
        }

        internal static IDictionary<string, string> Enumerate(this IEnumerable<string> self, string prefix)
        {
            return self.Select((item, i) => new { Item = item, Index = i })
                       .ToDictionary(k => $"{prefix}[{k.Index}]", v => v.Item);
        }
    }
}
