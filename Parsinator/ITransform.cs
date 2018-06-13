using System;
using System.Collections.Generic;

namespace Parsinator
{
    public interface ITransform
    {
        List<String> Transform(List<List<String>> allPages);
    }
}
