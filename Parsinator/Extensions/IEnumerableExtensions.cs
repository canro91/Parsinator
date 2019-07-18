using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public static class IEnumerableExtensions
    {
        public static IDictionary<String, String> Enumerate(this IEnumerable<String> self)
        {
            return self.Select((item, i) => new { Item = item, Index = i })
                       .ToDictionary(k => k.Index.ToString(), v => v.Item);
        }

        public static IDictionary<String, String> Enumerate(this IEnumerable<String> self, string prefix)
        {
            return self.Select((item, i) => new { Item = item, Index = i })
                       .ToDictionary(k => $"{prefix}[{k.Index}]", v => v.Item);
        }
    }
}
