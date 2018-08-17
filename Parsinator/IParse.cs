using System;
using System.Collections.Generic;

namespace Parsinator
{
    public interface IParse
    {
        String Key { get; }
        Int32? PageNumber { get; }
        bool HasMatched { get; }
        Func<String> Default { get; }
        IDictionary<String, String> Parse(String line, int lineNumber, int lineNumberFromBottom);
    }
}
