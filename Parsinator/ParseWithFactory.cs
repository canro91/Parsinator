using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public abstract class ParseWithFactory : IParse
    {
        private readonly Func<IDictionary<string, string>, string> _func;
        private readonly Func<string, string> _func2;

        protected ParseWithFactory(string key, int? pageNumber, Func<IDictionary<string, string>, string> factory, Func<string> @default)
        {
            Key = key;
            PageNumber = pageNumber;
            Default = @default;

            _func = factory;
        }

        protected ParseWithFactory(string key, Func<IDictionary<string, string>, string> factory, Func<string> @default)
        {
            Key = key;
            Default = @default;

            _func = factory;
        }

        protected ParseWithFactory(string key, Func<IDictionary<string, string>, string> factory)
        {
            Key = key;

            _func = factory;
        }

        protected ParseWithFactory(string key, Func<string, string> factory)
        {
            Key = key;

            _func2 = factory;
        }

        public string Key { get; private set; }
        public int? PageNumber { get; protected set; }
        public Func<string> Default { get; private set; }
        public bool HasMatched { get; protected set; }

        public bool HasFactory => _func != null || _func2 != null;

        protected string Factory(IDictionary<string, string> parsed)
        {
            try
            {
                return _func(parsed);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Found: {string.Join(",", parsed.Select((k, v) => $"[{k}]: {v}"))}", Key, e);
            }
        }

        protected string Factory(string parsed)
        {
            try
            {
                return _func2(parsed);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Found: {parsed}", Key, e);
            }
        }

        public abstract IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom);
    }
}
