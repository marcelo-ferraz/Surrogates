using Surrogates.Applications.ExecutingElsewhere;
using Surrogates.Expressions;
using Surrogates.Utilities;
using System;
using System.Security;
using System.Threading;

namespace Surrogates.Applications
{
    public static class ExecuteElsewhereMixins
    {
        private static long _domainIndex = 0;

        public static ElsewhereExpression<T> Call<T>(this ApplyExpression<T> self, Func<T, Delegate> method)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);

            return new ElsewhereExpression<T>(ext.Factory.Replace.This(method));
        }

        public static AndExpression<T> CallToOtherDomain<T, P>(this ApplyExpression<T> self, Func<T, Delegate> method, string domainName = null, SecurityZone securityZone = SecurityZone.MyComputer, params IPermission[] permissions)
        {
            if (!typeof(T).IsDefined(typeof(SerializableAttribute), true))
            { throw new ArgumentException("The surrogated type must be marked as Serializable"); }

            var ext =
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);

            var state =
                new ExecuteInOtherDomain.State
                {
                    Name = domainName ?? string.Concat("DynamicDomain_", Interlocked.Increment(ref _domainIndex)),
                    Permissions = permissions,
                    SecurityZone = securityZone
                };

            return ext
                .Factory
                .Replace
                .This(method)
                .Using<ExecuteInOtherDomain.Interceptor<P>>(i => (Func<ExecuteInOtherDomain.State, Delegate, P>) i.Execute)
                .And
                .AddProperty<ExecuteInOtherDomain.State>("State", state)
                .And
                .AddAttribute<SerializableAttribute>();
        }
    }
}
