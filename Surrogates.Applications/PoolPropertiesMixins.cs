
using Surrogates.Applications.Pooling;
using Surrogates.Expressions;
using Surrogates.Utilities;
using System;

namespace Surrogates.Applications
{
    public static class PoolPropertiesMixins
    {
        public static AndExpression<T> Pooling<T, P>(this ApplyExpression<T> self, Func<T, P> prop)
            where P: IDisposable
        {
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            //maps the type that will be dispatched
            ext.Container.Map(m => m
                .From<P>()
                .Visit
                .This(x => (Action) x.Dispose)
                .Using<PoolInterceptor<P>>(i => (Action<object, P>) i.Dispose));

            return ext
                .Factory
                .Replace
                .This(x => prop(x))
                .Accessors(a =>
                    a.Getter.Using<PoolInterceptor<P>>(i => (Func<dynamic, SurrogatesContainer, P>) i.Get));
        }
    }
}
