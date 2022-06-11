using System.Collections.Generic;
using System.Linq;

namespace Parsinator
{
    public class TransformFromSingleSkip : ITransform
    {
        private readonly ISkip ToSkip;

        public TransformFromSingleSkip(ISkip skip)
        {
            ToSkip = skip;
        }

        public IEnumerable<string> Transform(IEnumerable<IEnumerable<string>> allPages)
        {
            var details = ToSkip.Skip(allPages)
                                .SelectMany(t => t)
                                .ToList();

            return details;
        }
    }
}
