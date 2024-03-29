﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class Concatenate : IParse
    {
        private readonly string Separator;
        private readonly List<IParse> Parsers;

        private int? _lastParsedPage;
        private Dictionary<IParse, IDictionary<string, string>> _results;

        public Concatenate(string key, string separator, List<IParse> parsers)
        {
            Key = key;
            Separator = separator;
            Parsers = parsers;

            _lastParsedPage = null;
            _results = new Dictionary<IParse, IDictionary<string, string>>();
        }

        public string Key { get; private set; }
        public int? PageNumber
        {
            get
            {
                var withoutPage = Parsers.Where(t => !t.HasMatched && (t.PageNumber == null || t.PageNumber == 1));
                var withPages = Parsers.Where(t => !t.HasMatched && t.PageNumber.HasValue);

                if (withoutPage.Any()) return null;
                if (withPages.Any()) return withPages.Min(t => t.PageNumber);

                return _lastParsedPage;
            }
        }
        public Func<string> Default
        {
            get
            {
                var values = new List<string>();
                foreach (var p in Parsers)
                {
                    if (_results.ContainsKey(p))
                        values.AddRange(_results[p].Values);
                    else
                        values.Add(p.Default());
                }
                return () => string.Join(Separator, values);
            }
        }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            foreach (var p in Parsers.Where(t => !t.HasMatched))
            {
                var result = p.Parse(line, lineNumber, lineNumberFromBottom);
                if (p.HasMatched)
                {
                    _results.Add(p, result);
                    _lastParsedPage = p.PageNumber;
                }
            }
            return new Dictionary<string, string>();
        }
    }
}
