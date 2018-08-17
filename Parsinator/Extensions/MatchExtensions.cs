using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parsinator
{
    public static class MatchExtensions
    {
        public static IDictionary<String, String> Enumerate(this Match matches, Regex pattern)
        {
            return pattern.GetGroupNames().ToDictionary(k => k, v => matches.Groups[v].Value);
        }
    }
}
