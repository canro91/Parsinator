using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class AndThen : ParseWithFactory
    {
        private readonly IParse First;
        private readonly IParse Second;

        private bool _firstHasMatched;
        private IDictionary<string, string> _firstResult;

        public AndThen(String key, Func<IDictionary<string, string>, string> factory, IParse first, IParse second)
            : base(key, factory)
        {
            First = first;
            Second = second;
            PageNumber = first.PageNumber;
            HasMatched = false;

            _firstHasMatched = false;
            _firstResult = new Dictionary<string, string>();
        }

        public AndThen(Func<IDictionary<string, string>, String> factory, IParse first, IParse second)
            : this($"{first.Key}&{second.Key}", factory, first, second)
        {
        }

        public AndThen(IParse first, IParse second)
            : this($"{first.Key}&{second.Key}", null, first, second)
        {
        }


        public override IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
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
                    var accumulated = new Dictionary<String, String>(_firstResult).Merge(result2);
                    return (HasFactory)
                                ? new Dictionary<string, string> { { Key, Factory(accumulated) } }
                                : accumulated;
                }
            }
            return new Dictionary<string, string>();
        }
    }
}
