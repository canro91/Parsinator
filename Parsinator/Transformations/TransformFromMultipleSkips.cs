using System.Collections.Generic;
using System.Linq;

namespace Parsinator.Transformers
{
    public class TransformFromMultipleSkips : ITransform
    {
        private readonly IEnumerable<ISkip> ToSkip;

        public TransformFromMultipleSkips(IEnumerable<ISkip> skippers)
        {
            ToSkip = skippers;
        }

        public IEnumerable<string> Transform(IEnumerable<IEnumerable<string>> allPages)
        {
            var details = ToSkip.Chain(allPages)
                                .SelectMany(t => t)
                                .ToList();

            return details;
        }
    }
}
