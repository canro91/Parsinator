using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class AndThen : IParse
    {
        private readonly IParse First;
        private readonly IParse Second;
        private readonly Func<Tuple<IDictionary<string, string>, IDictionary<string, string>>, String> Factory;

        private bool _firstHasMatched;
        private IDictionary<string, string> _firstResult;

        public AndThen(String key, Func<Tuple<IDictionary<string, string>, IDictionary<string, string>>, String> factory, IParse first, IParse second)
        {
            Key = key;
            First = first;
            Second = second;
            PageNumber = first.PageNumber;
            Factory = factory;
            HasMatched = false;

            _firstHasMatched = false;
            _firstResult = new Dictionary<string, string>();
        }

        public AndThen(Func<Tuple<IDictionary<string, string>, IDictionary<string, string>>, String> factory, IParse first, IParse second)
            : this($"{first.Key}&{second.Key}", factory, first, second)
        {
        }

        public AndThen(IParse first, IParse second)
            : this($"{first.Key}&{second.Key}", null, first, second)
        {
        }


        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
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
                    if (Factory == null)
                    {
                        return new Dictionary<String, String>(_firstResult).Merge(result2);
                    }
                    else
                    {
                        var accumulated = new Tuple<IDictionary<string, string>, IDictionary<string, string>>(_firstResult, result2);
                        return new Dictionary<string, string> { { Key, Factory(accumulated) } };
                    }
                }
            }
            return new Dictionary<string, string>();
        }
    }
}
