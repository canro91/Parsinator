using System.Collections.Generic;

namespace Parsinator
{
    public interface ISkip
    {
        IEnumerable<IEnumerable<string>> Skip(IEnumerable<IEnumerable<string>> lines);
    }
}
