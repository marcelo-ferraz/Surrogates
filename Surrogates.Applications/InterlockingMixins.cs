using Surrogates.Applications.Interlocking;
using Surrogates.Expressions;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Runtime.Serialization;
using Surrogates.Tactics;
using Surrogates.Applications.Mixins;

namespace Surrogates.Applications
{
    public static class InterlockingMixins
    {
        public class InterlockedPair<T>
        { 
            public Func<T, Delegate> Reader { get; set; }
            public Func<T, Delegate> Writer { get; set; }
        }

        private static AndExpression<T> RW<T, I>(this ExpressionFactory<T> self, Func<T, object> prop)
            where I: InterlockedPropertyInterceptor
        {
            return self.Replace.This(prop)
                        .Accessors(m => m
                            .Getter.Using<I>(i => (Func<object>)i.Get)
                            .And
                            .Setter.Using<I>(i => (Action<object>)i.Set));
        }

        public static AndExpression<T> ReadAndWrite<T>(this ApplyExpression<T> self, params Func<T, object>[] properties)
        {            
            AndExpression<T> exp = null;

            Func<ExpressionFactory<T>> getExp = () =>
                (exp == null) ? self.PassOn().Factory : exp.And;

            foreach (var prop in properties)
            {
                exp = prop.GetPropType().IsValueType ?
                    getExp().RW<T, InterlockedValuePropertyInterceptor>(prop) :
                    getExp().RW<T, InterlockedRefPropertyInterceptor>(prop);
            }

            return exp;
        }

        public static AndExpression<T> ReadAndWrite<T>(this ApplyExpression<T> self, Func<T, Delegate> reader, Func<T, Delegate> writer)
        {
            return self.ReadAndWrite(new InterlockedPair<T>() { Reader = reader, Writer = writer });
        }

        public static AndExpression<T>  ReadAndWrite<T>(this ApplyExpression<T> self, params InterlockedPair<T>[] pairs)
        {
            T val = (T) FormatterServices
                .GetSafeUninitializedObject(typeof(T));

            AndExpression<T> exp = null;

            Func<ExpressionFactory<T>> getExp = 
                () =>
                    (exp == null) ? self.PassOn().Factory : exp.And;

            foreach (var pair in pairs)
            {
                exp = getExp()
                    .Replace
                    .This(pair.Reader)
                    .Using<InterlockedMethodInterceptor>("Read")
                    .And
                    .Replace
                    .This(pair.Writer)
                    .Using<InterlockedMethodInterceptor>("Write");
            }

            return exp;
        }
    }
}
