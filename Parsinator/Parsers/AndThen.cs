using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class AndThen : IParse
    {
        private readonly IParse First;
        private readonly IParse Second;

        private bool _firstHasMatched;
        private IDictionary<string, string> _firstResult;

        public AndThen(string key, IParse first, IParse second)
        {
            Key = key;

            First = first;
            Second = second;
            PageNumber = first.PageNumber;
            HasMatched = false;

            _firstHasMatched = false;
            _firstResult = new Dictionary<string, string>();
        }

        public AndThen(IParse first, IParse second)
            : this($"{first.Key}&{second.Key}", first, second)
        {
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            if (!_firstHasMatched)
            {
                var result1 = First.Parse(line, lineNumber, lineNumberFromBottom);
                if (First.HasMatched)
                {
                    _firstHasMatched = true;
                    _firstResult = result1;
                    PageNumber = Second.PageNumber;
                }
            }

            if (_firstHasMatched)
            {
                var result2 = Second.Parse(line, lineNumber, lineNumberFromBottom);
                if (Second.HasMatched)
                {
                    HasMatched = true;
                    return _firstResult.Merge(result2);
                }
            }

            return new Dictionary<string, string>();
        }
    }
}
