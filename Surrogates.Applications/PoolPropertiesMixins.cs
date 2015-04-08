
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
        public static AndExpression<T> Pooling<T>(this ApplyExpression<T> self, params Func<T, object>[] props)
            where T: IDisposable
        {
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            return ext
                .Factory.Replace.These(props)
                .Accessors(a =>
                    a.Getter.Using<PoolInterceptor<T>>(i => (Func<T>)i.Get))
                .And
                .Visit
                .This(obj => (Action)obj.Dispose)
                .Using<PoolInterceptor<T>>(i => (Action<T>)i.Dispose);
        }
    }
}
