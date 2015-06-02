using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;

namespace Surrogates.Applications.Mixins
{
    public static class ApplyExpressionMixins
    {
        [TargetedPatchingOptOut("This is just a lazy separation, to sweet the synthax up")]
        public static IExtension<T> PassOn<T>(this ApplyExpression<T> that)
        {
            var ext =
                new ShallowExtension<T>();

            return Pass.On(that, to: ext);            
        }

    }
}
