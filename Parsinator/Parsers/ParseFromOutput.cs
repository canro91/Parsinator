using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class ParseFromOutput : IParse
    {
        private readonly IParse Parser;
        private readonly IEnumerable<IParse> Parsers;

        public ParseFromOutput(IParse parseFrom, IEnumerable<IParse> parsers)
        {
            Parser = parseFrom;
            Parsers = parsers;
            PageNumber = parseFrom.PageNumber;
        }

        public String Key { get; private set; }
        public Int32? PageNumber { get; private set; }
        public Func<String> Default { get; private set; }
        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            var p = Parser.Parse(line, lineNumber, lineNumberFromBottom);
            if (Parser.HasMatched)
            {
                HasMatched = true;

                var input = p.Values;

                var innerParsers = new Dictionary<string, IEnumerable<IParse>>
                {
                    { "_", Parsers }
                };
                var parsinator = new Parser(innerParsers);
                var result = parsinator.Parse(new List<List<string>> { input.ToList() });

                return result.FirstOrDefault().Value ?? new Dictionary<string, string>();
            }
            return new Dictionary<string, string>();
        }
    }
}
