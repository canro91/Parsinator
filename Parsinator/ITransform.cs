using System.Collections.Generic;

namespace Parsinator
{
    public interface ITransform
    {
        IEnumerable<string> Transform(IEnumerable<IEnumerable<string>> allPages);
    }
}
