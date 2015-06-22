using Surrogates.Applications.Pooling;
using Surrogates.Expressions;
using Surrogates.Utilities;
using System;

namespace Surrogates.Applications
{
    public static class PoolPropertiesMixins
    {
        public static AndExpression<T> Pooling<T, P>(this ApplyExpression<T> self, Func<T, P> prop, int poolSize = 5, LoadingMode loadingMode = LoadingMode.Lazy,AccessMode accessMode = AccessMode.FIFO)
            where P: IDisposable
        {
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            string pooledKey =
                string.Concat(typeof(P).Name, '+', typeof(T).Name, "Pooled_", new Random(new Random().Next(new Random().Next())).Next());

            //maps the type that will be dispatched                        
            ext.Container.Map(m => m
                .From<P>()
                .Visit
                .This(x => (Action)x.Dispose)
                .Using<PoolInterceptor<P>.ToRelease>(i => (Action<object, P>)i.Dispose));

            return ext
                .Factory
                .Replace
                .This(x => prop(x))
                .Accessors(a =>
                    a.Getter.Using<PoolInterceptor<P>.ToGet>(interceptor: "Get"))
                .And
                .AddProperty<dynamic>("PoolState", 
                new { 
                    Size = poolSize, 
                    LoadingMode = loadingMode, 
                    AccessMode = accessMode, 
                    PooledKey = pooledKey 
                });
        }
    }
}
