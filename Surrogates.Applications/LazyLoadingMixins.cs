using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Surrogates.Applications.Utilities;
using Surrogates.Expressions.Accessors;
using Surrogates.Applications.LazyLoading;
using Surrogates.Applications.Model;

namespace Surrogates.Applications
{
    public static class LazyLoadingMixins
    {
        private static AndExpression<T> LazyLoading<T>(this InterferenceExpression<T> expr, ShallowExtension<T> ext, Func<string, T, object> loader)
        {            
            var loadersProp = ext
                .Strategies
                .NewProperties.Find(p => p.Name == "Loaders");
            
            var loaders = Pass
                .Current<Strategy.ForProperties>(expr)
                .MergeProperty("Loaders", p => (Func<string, T, object>)loader);

            return expr
                .Accessors(a => a
                    .Getter.Using<LazyLoadingInterceptor<T>>("LazyLoadingInterceptor", i => (Func<string, T, Dictionary<string, Func<string, T, object>>, object>)i.Load)
                    .And
                    .Setter.Using<LazyLoadingInterceptor<T>>("LazyLoadingInterceptor", i => (Action<string, T, object>)i.MarkAsDirty))
                .And
                .AddProperty("Loaders", loaders)
                .And
                .AddInterface<IContainsLazyLoadings>()
                .And
                .AddProperty<ILazyLoadingInterceptor>("LazyLoadingInterceptor", new LazyLoadingInterceptor<T>());
        }

        public static AndExpression<T> LazyLoading<T>(this ApplyExpression<T> that, Func<T, object> prop, Func<string, T, object> loader)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .This(prop)
                .LazyLoading(ext, loader);
        }

        public static AndExpression<T> LazyLoading<T>(this ApplyExpression<T> that, Func<T, object>[] props, Func<string, T, object> loader)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .These(props)
                .LazyLoading(ext, loader);
        }
        public static AndExpression<T> LazyLoading<T>(this ApplyExpression<T> that, string prop, Func<string, T, object> loader)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .Property(prop)
                .LazyLoading(ext, loader);
        }

        public static AndExpression<T> LazyLoading<T>(this ApplyExpression<T> that, string[] props, Func<string, T, object> loader)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .Properties(props)
                .LazyLoading(ext, loader);
        }
    }
}
