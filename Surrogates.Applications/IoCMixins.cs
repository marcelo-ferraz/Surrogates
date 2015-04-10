using Surrogates.Expressions;
using Surrogates.Utilities;
using System;

namespace Surrogates.Applications
{
    public static class IoCMixins
    {
        public class IoCInterceptor4<T>
        { 
            private T _value;

            public T Get()
            {
                return object.ReferenceEquals(_value, default(T)) ?
                    (_value = Activator.CreateInstance<T>()) :
                    _value;
            }

            public void Set(T s_value)
            {
                _value = s_value;
            }
        }

        public static AndExpression<T> IoC<T>(this ApplyExpression<T> self, params Func<T, object>[] properties)
        { 
            var ext = new ShallowExtension<T>();
            Pass.On<T>(self, ext);

            return ext.Factory.Replace.These(properties).Accessors(
                m => m
                    .Getter.Using<IoCInterceptor4<T>>(ioc => (Func<T>)ioc.Get)
                    .And
                    .Setter.Using<IoCInterceptor4<T>>(ioc => (Action<T>)ioc.Set));
        }
    }
}
