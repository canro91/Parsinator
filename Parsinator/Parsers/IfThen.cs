using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class IfThen : IParse
    {
        private readonly Func<string, bool> Predicate;
        private readonly IParse If;
        private readonly IParse Then;
        private readonly IParse Else;

        private Dictionary<string, string> _output;
        private bool _wasEvaluated;
        private bool _predicateHolds;

        public IfThen(Func<String, bool> predicate, IParse @if, IParse then, IParse @else)
        {
            this.Predicate = predicate;
            this.If = @if;
            this.Then = then;
            this.Else = @else;
            _wasEvaluated = false;
            _predicateHolds = false;
            _output = new Dictionary<string, string>();
        }

        public IfThen(Func<string, bool> predicate, IParse @if, IParse then)
            : this(predicate, @if, then, null)
        {
        }

        public string Key { get; private set; }
        public int? PageNumber { get; private set; }
        public Func<string> Default
        {
            get
            {
                if (!_wasEvaluated)
                    return null;

                // In case of, neither then nor else parsers parse, the value
                // parsed from if is returned.
                Key = If.Key;
                return () => _output.FirstOrDefault().Value;
            }
        }

        public bool HasMatched { get; private set; }

        public IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom)
        {
            if (!_wasEvaluated)
            {
                var p = If.Parse(line, lineNumber, lineNumberFromBottom);
                if (If.HasMatched)
                {
                    _wasEvaluated = true;
                    _predicateHolds = Predicate(p.Values.FirstOrDefault());

                    _output.Merge(p);
                }
            }
            if (_wasEvaluated && _predicateHolds)
            {
                var result = Then.Parse(line, lineNumber, lineNumberFromBottom);
                if (Then.HasMatched)
                {
                    _output.Merge(result);

                    HasMatched = true;
                    return _output;
                }
            }
            if (_wasEvaluated && !_predicateHolds && Else != null)
            {
                var result = Else.Parse(line, lineNumber, lineNumberFromBottom);
                if (Else.HasMatched)
                {
                    _output.Merge(result);

                    HasMatched = true;
                    return _output;
                }
            }

            return new Dictionary<string, string>();
        }
    }
}
