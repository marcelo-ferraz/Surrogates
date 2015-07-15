using Surrogates.Applications.Utilities;
using Surrogates.Applications.Interlocking;
using Surrogates.Expressions;
using Surrogates.Utilities.Mixins;
using System;
using System.Runtime.Serialization;
using Surrogates.Utilities;
using Surrogates.Applications.Model;

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
        
        /// <summary>
        /// Applies Read and write locks on the call of this properties. On get will be applied a read lock and on set the write lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static AndExpression<T> ReadAndWrite<T>(this ApplyExpression<T> self, Func<T, object> property)
        {
            AndExpression<T> exp = null;

            Func<ExpressionFactory<T>> getExp = () =>
                (exp == null) ? Pass.On<T, ShallowExtension<T>>(self).Factory : exp.And;

            return property.GetPropType().IsValueType ?
                    getExp().RW<T, InterlockedValuePropertyInterceptor>(property) :
                    getExp().RW<T, InterlockedRefPropertyInterceptor>(property);
        }

        /// <summary>
        /// Applies Read and write locks on the call of those properties. On get will be applied a read lock and on set the write lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static AndExpression<T> ReadAndWrite<T>(this ApplyExpression<T> self, Func<T, object>[] properties)
        {            
            AndExpression<T> exp = null;

            Func<ExpressionFactory<T>> getExp = () =>
                (exp == null) ? Pass.On<T, ShallowExtension<T>>(self).Factory : exp.And;

            foreach (var prop in properties)
            {
                exp = prop.GetPropType().IsValueType ?
                    getExp().RW<T, InterlockedValuePropertyInterceptor>(prop) :
                    getExp().RW<T, InterlockedRefPropertyInterceptor>(prop);
            }

            return exp;
        }

        /// <summary>
        /// Applies Read and write locks on the call of those properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static AndExpression<T> ReadAndWrite<T>(this ApplyExpression<T> self, Func<T, Delegate> reader, Func<T, Delegate> writer)
        {
            return self.ReadAndWrite(new InterlockedPair<T>() { Reader = reader, Writer = writer });
        }

        /// <summary>
        /// Applies Read and write locks on the call of those properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public static AndExpression<T>  ReadAndWrite<T>(this ApplyExpression<T> self, params InterlockedPair<T>[] pairs)
        {
            T val = (T) FormatterServices
                .GetSafeUninitializedObject(typeof(T));

            AndExpression<T> exp = null;

            Func<ExpressionFactory<T>> getExp = 
                () =>
                    (exp == null) ? Pass.On<T, ShallowExtension<T>>(self).Factory : exp.And;

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
