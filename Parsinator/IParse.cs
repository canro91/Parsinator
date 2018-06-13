using System;
using System.Collections.Generic;

namespace Parsinator
{
    public interface IParse
    {
        String Key { get; }
        bool HasMatched { get; }
        Func<String> Default { get; }
        KeyValuePair<String, String> Parse(String line, int lineNumber, int lineNumberFromBottom);
    }
}
