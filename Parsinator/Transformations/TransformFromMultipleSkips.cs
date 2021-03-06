﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsinator.Transformers
{
    public class TransformFromMultipleSkips : ITransform
    {
        private readonly IList<ISkip> ToSkip;

        public TransformFromMultipleSkips(IList<ISkip> skippers)
        {
            ToSkip = skippers;
        }

        public List<string> Transform(List<List<string>> allPages)
        {
            List<String> details = ToSkip.Chain(allPages)
                                         .SelectMany(t => t)
                                         .ToList();
            return details;
        }
    }
}
