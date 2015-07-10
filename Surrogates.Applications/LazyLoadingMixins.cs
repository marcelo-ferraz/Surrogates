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
        private static AndExpression<T> LazyLoading<T, TProp>(this InterferenceExpression<T> expr, ShallowExtension<T> ext, Func<string, T, TProp> loader)
        {
            var loaderProp =
                ext.Strategies.NewProperties.Find(np => np.Name == "Loaders");

            var loadersProp = ext
                .Strategies
                .NewProperties.Find(p => p.Name == "Loaders");
            
            var strat = Pass.Current<Strategy.ForProperties>(expr);

            var newValues = strat
                .Properties
                .ToDictionary(p => p.Original.Name, p => (Func<string, T, TProp>)loader);

            var loaders =
                loadersProp != null ?
                ((Dictionary<string, Func<string, T, TProp>>)loadersProp.DefaultValue).MergeLeft(newValues) :
                newValues;

            return expr
                .Accessors(a => a
                    .Getter.Using<LazyLoadingInterceptor<T, TProp>>("LazyLoadingInterceptor", i => (Func<string, T, Dictionary<string, Func<string, T, TProp>>, TProp>)i.Load)
                    .And
                    .Setter.Using<LazyLoadingInterceptor<T, TProp>>("LazyLoadingInterceptor", i => (Action<string, T, TProp>)i.MarkAsDirty))
                .And
                .AddProperty("Loaders", loaders)
                .And
                .AddInterface<IContainsLazyLoadings<T, TProp>>();
        }

        public static AndExpression<T> LazyLoading<T, TProp>(this ApplyExpression<T> that, Func<T, object> prop, Func<string, T, TProp> loader)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .This(prop)
                .LazyLoading(ext, loader);
        }

        public static AndExpression<T> LazyLoading<T, TProp>(this ApplyExpression<T> that, Func<T, object>[] props, Func<string, T, TProp> loader)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .These(props)
                .LazyLoading(ext, loader);
        }
        public static AndExpression<T> LazyLoading<T, TProp>(this ApplyExpression<T> that, string prop, Func<string, T, TProp> loader)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .Property(prop)
                .LazyLoading(ext, loader);
        }

        public static AndExpression<T> LazyLoading<T, TProp>(this ApplyExpression<T> that, string[] props, Func<string, T, TProp> loader)
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
