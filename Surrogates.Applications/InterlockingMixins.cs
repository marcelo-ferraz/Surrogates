using Surrogates.Applications.Interlocking;
using Surrogates.Expressions;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Runtime.Serialization;
using Surrogates.Tactics;

namespace Surrogates.Applications
{
    public static class InterlockingMixins
    {
        private static AndExpression<T> RW<T, I>(this IExtension<T> self, Func<T, object> prop)
            where I: InterlockedPropertyInterceptor
        {
            return self.Factory.Replace.This(prop)
                        .Accessors(m => m
                            .Getter.Using<I>(i => (Func<object>)i.Get)
                            .And
                            .Setter.Using<I>(i => (Action<object>)i.Set));
        }
        public static AndExpression<T> ReadAndWrite<T>(this ApplyExpression<T> self, params Func<T, object>[] properties)
        {
            var ext = 
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);

            AndExpression<T> exp = null;

            foreach (var prop in properties)
            {
                exp = prop.GetPropType().IsValueType ?
                    ext.RW<T, InterlockedValuePropertyInterceptor>(prop) :
                    ext.RW<T, InterlockedRefPropertyInterceptor>(prop);
            }

            return exp;
        }

        public static AndExpression<T>  ReadAndWrite<T>(this ApplyExpression<T> self, Func<T, Delegate> reader, Func<T, Delegate> writer)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);
            //var current = Pass.Current<Strategy.ForMethods>(ext.Factory.Replace);

            T val = (T) FormatterServices
                .GetSafeUninitializedObject(typeof(T)); 

            var and =
            //    reader(val).Method.ReturnType == typeof(void) ?
            //    ext.Factory.Replace.This(reader).Using<InterlockedActionInterceptor>("Read").And :
                ext.Factory.Replace.This(reader).Using<InterlockedFuncInterceptor>("Read").And;
            //ext.Factory.Replace.This(reader).Using<InterlockedFuncInterceptor>(i => (Func<Delegate, object[], object>)i.Read).And;

            return and.Replace.This(writer).Using<InterlockedMethodInterceptor>("Write");
        }
    }
}
