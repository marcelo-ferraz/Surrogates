using Surrogates.Applications.Pooling;
using Surrogates.Expressions;
using Surrogates.Expressions.Accessors;
using Surrogates.Tactics;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Surrogates.Applications.Utilities;
using Surrogates.Applications.Model;

namespace Surrogates.Applications
{
    public static class PoolPropertiesMixins
    {
        private static AndExpression<T> ApplyPool<T, P>(this InterferenceExpression<T> expr, int poolSize, LoadingMode loadingMode, AccessMode accessMode, ShallowExtension<T> ext)
            where P : IDisposable
        {
            string pooledKey =
                string.Concat(typeof(P).Name, '+', typeof(T).Name, "Pooled_", new Random(new Random().Next(new Random().Next())).Next());

            //maps the type that will be dispatched                        

            if (!ext.Container.Has<P>())
            {
                ext.Container.Map(m => m
                    .From<P>()
                    .Visit
                    .This(x => (Action)x.Dispose)
                    .Using<PoolInterceptor<P>.ToRelease>(i => (Action<P, Pool<P>>)i.Dispose)
                    .And
                    .AddProperty<Pool<T>>("Pool")
                    .And
                    .AddInterface<IContainsPool<P>>());
            }
            
            var defaults = Pass
                .Current<Strategy.ForProperties>(expr)
                .MergeProperty<dynamic>("PoolStates", new
                {
                    Size = poolSize,
                    LoadingMode = loadingMode,
                    AccessMode = accessMode,
                    PooledKey = pooledKey
                });
            
            return expr
                .Accessors(a =>
                    a.Getter.Using<PoolInterceptor<P>.ToGet>(interceptor: "Get"))
                .And
                .AddProperty<Dictionary<string, dynamic>>("PoolStates", defaults);
        }

        public static AndExpression<T> Pooling<T, P>(this ApplyExpression<T> self, string[] props, int poolSize = 5, LoadingMode loadingMode = LoadingMode.Lazy, AccessMode accessMode = AccessMode.FIFO)
               where P : IDisposable
        {
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            return ext
                .Factory
                .Replace
                .Properties(props)
                .ApplyPool<T, P>(poolSize, loadingMode, accessMode, ext);
        }
        
        public static AndExpression<T> Pooling<T, P>(this ApplyExpression<T> self, string prop, int poolSize = 5, LoadingMode loadingMode = LoadingMode.Lazy, AccessMode accessMode = AccessMode.FIFO)
               where P : IDisposable
        {
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            return ext
                .Factory
                .Replace
                .Property(prop)
                .ApplyPool<T, P>(poolSize, loadingMode, accessMode, ext);
        }

        public static AndExpression<T> Pooling<T, P>(this ApplyExpression<T> self, Func<T, P>[] props, int poolSize = 5, LoadingMode loadingMode = LoadingMode.Lazy, AccessMode accessMode = AccessMode.FIFO)
               where P : IDisposable
        {
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            return ext
                .Factory
                .Replace
                .These(x => props.Select(p => p).ToArray())
                .ApplyPool<T, P>(poolSize, loadingMode, accessMode, ext);
        }

        public static AndExpression<T> Pooling<T, P>(this ApplyExpression<T> self, Func<T, P> prop, int poolSize = 5, LoadingMode loadingMode = LoadingMode.Lazy, AccessMode accessMode = AccessMode.FIFO)
            where P : IDisposable
        {
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            return ext
                .Factory
                .Replace
                .This(x => prop(x))
                .ApplyPool<T, P>(poolSize, loadingMode, accessMode, ext);
        }
    }
}
