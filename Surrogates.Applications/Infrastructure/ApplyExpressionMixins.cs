using Surrogates.Expressions;
using Surrogates.Utilities;
using System.Runtime;

namespace Surrogates.Applications.Infrastructure
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
