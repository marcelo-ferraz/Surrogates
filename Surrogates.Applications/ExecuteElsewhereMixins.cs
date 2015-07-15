using Surrogates.Applications.ExecutingElsewhere;
using Surrogates.Applications.Model;
using Surrogates.Expressions;
using Surrogates.Model.Entities;
using Surrogates.Utilities;
using System;

namespace Surrogates.Applications
{
    public static class ExecuteElsewhereMixins
    {
        private static ElsewhereExpression<T> Calls<T>(ApplyExpression<T> self, Func<ShallowExtension<T>, UsingInterferenceExpression<T>> getExpr)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);
            ext.Strategies.Accesses = ~Access.Container;

            return new ElsewhereExpression<T>(getExpr(ext));
        }

        /// <summary>
        /// Change the nature of the call of this prop
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="self">The expression</param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static ElsewhereExpression<T> Calls<T>(this ApplyExpression<T> self, Func<T, Delegate> method)
        {
            return Calls(self, ext => ext.Factory.Replace.This(method));
        }

        /// <summary>
        /// Change the nature of the call of this prop
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="self">The expression</param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static ElsewhereExpression<T> Calls<T>(this ApplyExpression<T> self, Func<T, Delegate>[] methods)
        {
            return Calls(self, ext => ext.Factory.Replace.These(methods));
        }

        /// <summary>
        /// Change the nature of the call of this prop
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="self">The expression</param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static ElsewhereExpression<T> Calls<T>(this ApplyExpression<T> self, string method)
        {
            return Calls(self, ext => ext.Factory.Replace.Method(method));
        }

        /// <summary>
        /// Change the nature of the call of this prop
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="self">The expression</param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static ElsewhereExpression<T> Calls<T>(this ApplyExpression<T> self, string[] methods)
        {
            return Calls(self, ext => ext.Factory.Replace.Methods(methods));
        }
    }
}