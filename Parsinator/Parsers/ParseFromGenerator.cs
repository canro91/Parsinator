using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class ParseFromGenerator<T> : IParse
    {
        private readonly Func<T, T> Next;
        private T _current;

        public ParseFromGenerator(String key, T seed, Func<T, T> next)
        {
            this.Key = key;
            this.Default = null;
            this.HasMatched = true;
            this.Next = next;
            this._current = seed;
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            _current = Next(_current);
            return new Dictionary<string, string> { { Key, _current.ToString() } };
        }
    }
}
