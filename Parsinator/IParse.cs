using System;
using System.Collections.Generic;

namespace Parsinator
{
    public interface IParse
    {
        string Key { get; }
        int? PageNumber { get; }
        bool HasMatched { get; }
        Func<string> Default { get; }
        IDictionary<string, string> Parse(string line, int lineNumber, int lineNumberFromBottom);
    }
}
