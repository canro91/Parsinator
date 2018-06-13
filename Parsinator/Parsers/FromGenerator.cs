using System;
using System.Collections.Generic;

namespace Parsinator
{
    public class FromGenerator<T> : IParse
    {
        private readonly Func<T, T> Next;
        private T _current;

        public FromGenerator(String key, T seed, Func<T, T> next)
        {
            this.Key = key;
            this.Default = null;
            this.HasMatched = true;
            this.Next = next;
            this._current = seed;
        }

        public String Key { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public KeyValuePair<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            _current = Next(_current);
            return new KeyValuePair<string, string>(Key, _current.ToString());
        }
    }
}
