using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public static class MatchExtensions
    {
        internal static IDictionary<string, string> Enumerate(this Match matches, Regex pattern)
        {
            return pattern.GetGroupNames().ToDictionary(k => k, v => matches.Groups[v].Value);
        }
    }
}
