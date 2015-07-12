using Surrogates.Expressions.Accessors;
using Surrogates.Utilities;
using System;
using Surrogates.Expressions;
using Surrogates.Applications.IoC;

namespace Surrogates.Applications
{
    public static class IoCMixins
    {
        public static IoCExpression<T> IoCFor<T>(this ApplyExpression<T> self, params Func<T, object>[] properties)
        {
            return new IoCExpression<T>(Pass.On<T, ShallowExtension<T>>(self).Factory.Replace.These(properties));
        }

        public static IoCExpression<T> IoCFor<T>(this ApplyExpression<T> self, Func<T, object> property)
        {
            return new IoCExpression<T>(Pass.On<T, ShallowExtension<T>>(self).Factory.Replace.This(property));
        }

        public static IoCExpression<T> IoCFor<T>(this ApplyExpression<T> self, params string[] properties)
        {
            return new IoCExpression<T>(Pass.On<T, ShallowExtension<T>>(self).Factory.Replace.Properties(properties));
        }

        public static IoCExpression<T> IoCFor<T>(this ApplyExpression<T> self, string property)
        {
            return new IoCExpression<T>(Pass.On<T, ShallowExtension<T>>(self).Factory.Replace.Property(property));
        }
    }
}