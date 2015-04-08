
using Surrogates.Applications.Pooling;
using Surrogates.Expressions;
using Surrogates.Expressions.Accessors;
using Surrogates.Tactics;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications
{
    public static class PoolPropertiesMixins
    {
        public static AndExpression<T> Pooling<T, P>(this ApplyExpression<T> self, Func<T, P> prop)
            where P: IDisposable
        {
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            return ext
                .Factory
                .Replace
                .This(x => prop(x))
                .Accessors(a =>
                    a.Getter.Using<PoolInterceptor<P>>(i => (Func<P>)i.Get));
        }
    }
}
