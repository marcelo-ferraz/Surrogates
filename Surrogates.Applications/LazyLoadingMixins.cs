using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Surrogates.Applications.Infrastructure;
using Surrogates.Expressions.Accessors;
using Surrogates.Applications.LazyLoading;

namespace Surrogates.Applications
{
    public static class LazyLoadingMixins
    {
        private static AndExpression<T> LazyLoading<T>(this InterferenceExpression<T> expr, ShallowExtension<T> ext, Func<string, T, object> loader)
        {
            var loaderProp =
                ext.Strategies.NewProperties.Find(np => np.Name == "Loaders");

            var loadersProp = ext
                .Strategies
                .NewProperties.Find(p => p.Name == "Loaders");
            
            var strat = Pass.Current<Strategy.ForProperties>(expr);

            var newValues = strat
                .Properties
                .ToDictionary(p => p.Original.Name, p => (Func<string, T, object>)loader);

            var loaders =
                loadersProp != null ?
                ((Dictionary<string, Func<string, T, object>>)loadersProp.DefaultValue).MergeLeft(newValues) :
                newValues;

            return expr
                .Accessors(a => a
                    .Getter.Using<LazyLoadingInterceptor<T>>("LazyLoadingInterceptor", i => (Func<string, T, Dictionary<string, Func<string, T, object>>, object>)i.Load)
                    .And
                    .Setter.Using<LazyLoadingInterceptor<T>>("LazyLoadingInterceptor", i => (Action<string, T, object>)i.MarkAsDirty))
                .And
                .AddProperty("Loaders", loaders)
                .And
                .AddInterface<IContainsLazyLoadings<T>>()
                .And
                .AddProperty("LazyLoadingInterceptor", new LazyLoadingInterceptor<T>())
                ;
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
