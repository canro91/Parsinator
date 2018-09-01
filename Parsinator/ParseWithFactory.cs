using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public abstract class ParseWithFactory : IParse
    {
        private readonly Func<IDictionary<String, String>, String> _func;
        private readonly Func<String, String> _func2;

        protected ParseWithFactory(string key, int? pageNumber, Func<IDictionary<String, String>, String> factory, Func<string> @default)
        {
            Key = key;
            PageNumber = pageNumber;
            Default = @default;

            _func = factory;
        }

        protected ParseWithFactory(string key, Func<IDictionary<String, String>, String> factory, Func<string> @default)
        {
            Key = key;
            Default = @default;

            _func = factory;
        }

        protected ParseWithFactory(string key, Func<IDictionary<String, String>, String> factory)
        {
            Key = key;

            _func = factory;
        }

        protected ParseWithFactory(string key, Func<String, String> factory)
        {
            Key = key;

            _func2 = factory;
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; protected set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; protected set; }

        public bool HasFactory => _func != null || _func2 != null;

        protected String Factory(IDictionary<String, String> parsed)
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

        protected String Factory(String parsed)
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
