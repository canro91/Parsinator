using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class AndThen : IParse
    {
        private readonly IParse First;
        private readonly IParse Second;
        private readonly Func<Tuple<String, String>, String> Factory;

        private bool _firstHasMatched;
        private KeyValuePair<string, string> _firstResult;

        public AndThen(String key, Func<Tuple<String, String>, String> factory, IParse first, IParse second)
        {
            Key = key;
            First = first;
            Second = second;
            PageNumber = first.PageNumber;
            Factory = factory;
            HasMatched = false;

            _firstHasMatched = false;
            _firstResult = new KeyValuePair<string, string>();
        }

        public AndThen(Func<Tuple<String, String>, String> factory, IParse first, IParse second)
            : this($"{first.Key}&{second.Key}", factory, first, second)
        {
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            if (!_firstHasMatched)
            {
                var result1 = First.Parse(line, lineNumber, lineNumberFromBottom);
                if (result1.Key != null)
                {
                    _firstHasMatched = true;
                    _firstResult = result1;
                    PageNumber = Second.PageNumber;
                }
            }
            else
            {
                var result2 = Second.Parse(line, lineNumber, lineNumberFromBottom);
                if (result2.Key != null)
                {
                    HasMatched = true;
                    var accumulated = new Tuple<string, string>(_firstResult.Value, result2.Value);
                    return new KeyValuePair<string, string>(Key, Factory(accumulated));
                }
            }
            return new KeyValuePair<string, string>();
        }
    }
}
