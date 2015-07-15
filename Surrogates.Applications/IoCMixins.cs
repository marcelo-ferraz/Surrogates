using Surrogates.Expressions.Accessors;
using Surrogates.Utilities;
using System;
using Surrogates.Expressions;
using Surrogates.Applications.IoC;
using Surrogates.Applications.Model;

namespace Surrogates.Applications
{
    public static class IoCMixins
    {
        /// <summary>
        /// Exposes properties in which a value will be injected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static IoCExpression<T> IoCFor<T>(this ApplyExpression<T> self, params Func<T, object>[] properties)
        {
            return new IoCExpression<T>(Pass.On<T, ShallowExtension<T>>(self).Factory.Replace.These(properties));
        }

        /// <summary>
        /// Exposes the property in which a value will be injected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IoCExpression<T> IoCFor<T>(this ApplyExpression<T> self, Func<T, object> property)
        {
            return new IoCExpression<T>(Pass.On<T, ShallowExtension<T>>(self).Factory.Replace.This(property));
        }

        /// <summary>
        /// Exposes properties in which a value will be injected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static IoCExpression<T> IoCFor<T>(this ApplyExpression<T> self, params string[] properties)
        {
            return new IoCExpression<T>(Pass.On<T, ShallowExtension<T>>(self).Factory.Replace.Properties(properties));
        }

        /// <summary>
        /// Exposes the property in which a value will be injected
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IoCExpression<T> IoCFor<T>(this ApplyExpression<T> self, string property)
        {
            return new IoCExpression<T>(Pass.On<T, ShallowExtension<T>>(self).Factory.Replace.Property(property));
        }
    }
}