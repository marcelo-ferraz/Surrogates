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

            //private MockedRepository _repository = new MockedRepository();

            private static Func<string, T> Get { get; set; }

            public T Load(string s_name)
            {
                return object.Equals(_value, default(T)) ?
                    (_value = Get(s_name)) :
                    _value;
            }

            public void MarkAsDirty(T s_value)
            {
                _isDirty = true;
                _value = s_value;
            }
        }

        public static AndExpression<T> LazyLoading<T, P>(this ApplyExpression<T> that, Func<T, object> prop)
        {
            var ext = new ShallowExtension<T>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .This(prop)
                .Accessors(a => a
                    .Getter.Using<IdLazyLoaderInterceptor<P>>("idLoader", i => (Func<string, P>) i.Load)
                    .And
                    .Setter.Using<IdLazyLoaderInterceptor<P>>("idLoader", i => (Action<P>) i.MarkAsDirty));
        }
    }
}
