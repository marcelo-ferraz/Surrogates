using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Surrogates.Aspects.Utilities;
using Surrogates.Expressions.Accessors;
using Surrogates.Aspects.LazyLoading;
using Surrogates.Aspects.Model;

namespace Surrogates.Aspects
{
    public static class LazyLoadingMixins
    {
        private static AndExpression<T> LazyLoading<T>(this InterferenceExpression<T> expr, ShallowExtension<T> ext, Func<string, T, object> loader)
        {            
            var loadersProp = ext
                .Strategies
                .NewProperties.Find(p => p.Name == "Loaders");

            var loaders = InternalsInspector
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

        /// <summary>
        /// Applies a lazy loading behavior to a given properties
        /// </summary>
        /// <typeparam name="T">The type which contains that properties</typeparam>
        /// <param name="that">The previous expression</param>
        /// <param name="prop">The properties</param>
        /// <param name="loader">The loader of that properties</param>
        /// <returns></returns>
        public static AndExpression<T> LazyLoading<T>(this ApplyExpression<T> that, Func<T, object> prop, Func<string, T, object> loader)
        {
            var ext = new ShallowExtension<T>();
            InternalsInspector.GetInternals(that, ext);

            return ext
                .Factory
                .Replace
                .This(prop)
                .LazyLoading(ext, loader);
        }

        /// <summary>
        /// Applies a lazy loading behavior to a given set of properties
        /// </summary>
        /// <typeparam name="T">The type which contains that properties</typeparam>
        /// <param name="that">The previous expression</param>
        /// <param name="forgetProp">Those properties</param>
        /// <param name="loader">The loader of those properties</param>
        /// <returns></returns>
        public static AndExpression<T> LazyLoading<T>(this ApplyExpression<T> that, Func<T, object>[] props, Func<string, T, object> loader)
        {
            var ext = new ShallowExtension<T>();
            InternalsInspector.GetInternals(that, ext);

            return ext
                .Factory
                .Replace
                .These(props)
                .LazyLoading(ext, loader);
        }
        
        /// <summary>
        /// Applies a lazy loading behavior to a given properties
        /// </summary>
        /// <typeparam name="T">The type which contains that properties</typeparam>
        /// <param name="that">The previous expression</param>
        /// <param name="prop">The properties's name</param>
        /// <param name="loader">The loader of that properties</param>
        /// <returns></returns>
        public static AndExpression<T> LazyLoading<T>(this ApplyExpression<T> that, string prop, Func<string, T, object> loader)
        {
            var ext = new ShallowExtension<T>();
            InternalsInspector.GetInternals(that, ext);

            return ext
                .Factory
                .Replace
                .Property(prop)
                .LazyLoading(ext, loader);
        }

        /// <summary>
        /// Applies a lazy loading behavior to a given set of properties
        /// </summary>
        /// <typeparam name="T">The type which contains that properties</typeparam>
        /// <param name="that">The previous expression</param>
        /// <param name="forgetProp">Those properties</param>
        /// <param name="loader">The loader of those properties</param>
        /// <returns></returns>
        public static AndExpression<T> LazyLoading<T>(this ApplyExpression<T> that, string[] props, Func<string, T, object> loader)
        {
            var ext = new ShallowExtension<T>();
            InternalsInspector.GetInternals(that, ext);

            return ext
                .Factory
                .Replace
                .Properties(props)
                .LazyLoading(ext, loader);
        }
    }
}
