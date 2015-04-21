using Surrogates.Expressions;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications
{
    public static class LazyLoadingMixins
    {
        public class IdLazyLoaderInterceptor<T>
        {
            private T _value;
            private bool _isDirty = false;

            public T Load(string s_name, Func<string, T> s_Getter)
            {
                return object.Equals(_value, default(T)) ?
                    (_value = s_Getter(s_name)) :
                    _value;
            }

            public void MarkAsDirty(T s_value)
            {
                _isDirty = true;
                _value = s_value;
            }
        }

        public static AndExpression<T> LazyLoading<T, R>(this ApplyExpression<T> that, Func<T, object> prop, Func<string, R> howToGet)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .AddProperty("Getter", howToGet)
                .And
                .Replace
                .This(prop)
                .Accessors(a => a
                    .Getter.Using<IdLazyLoaderInterceptor<R>>("idLoader", i => (Func<string, Func<string, R>, R>) i.Load)
                    .And
                    .Setter.Using<IdLazyLoaderInterceptor<R>>("idLoader", i => (Action<R>) i.MarkAsDirty));
        }
    }
}
