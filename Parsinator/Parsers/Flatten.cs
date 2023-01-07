using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class Flatten : IParse
    {
        private readonly IParse Inner;
        private readonly Func<IDictionary<string, string>, string> Func;

        public Flatten(Func<IDictionary<string, string>, string> func, IParse inner)
        {
            Key = inner.Key;
            PageNumber = inner.PageNumber;
            Default = inner.Default;

            Func = func;
            Inner = inner;
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var result = Inner.Parse(line, lineNumber, lineNumberFromBottom);
            if (!Inner.HasMatched)
            {
                PageNumber = Inner.PageNumber != PageNumber
                                ? Inner.PageNumber
                                : PageNumber;

                return new Dictionary<string, string>();
            }

            try
            {
                HasMatched = true;
                return new Dictionary<string, string> { { Key, Func(result) } };
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Found: {string.Join(",", result.Select((k, v) => $"[{k}]: {v}"))}", Key, e);
            }
        }
    }
}