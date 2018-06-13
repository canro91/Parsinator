using System;
using System.Collections.Generic;

namespace Parsinator
{
    public interface ISkip
    {
        List<List<String>> Skip(List<List<String>> lines);
    }
}
