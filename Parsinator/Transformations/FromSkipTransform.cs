using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class FromSkipTransform : ITransform
    {
        private readonly ISkip ToSkip;

        public FromSkipTransform(ISkip skip)
        {
            this.ToSkip = skip;
        }

        public List<string> Transform(List<List<string>> allPages)
        {
            List<String> details = ToSkip.Skip(allPages)
                                         .SelectMany(t => t)
                                         .ToList();
            return details;
        }
    }
}
