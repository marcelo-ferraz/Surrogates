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

        public static ElsewhereExpression<T> Call<T>(this ApplyExpression<T> self, params Func<T, Delegate>[] methods)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);

            return new ElsewhereExpression<T>(ext.Factory.Replace.These(methods));
        }

        public static AndExpression<T> CallToOtherDomain<T, P>(this ApplyExpression<T> self, Func<T, Delegate> method, string domainName = null, SecurityZone securityZone = SecurityZone.MyComputer, params IPermission[] permissions)
        {

            var ext =
                new ShallowExtension<T>();

            Pass.On<T>(self, ext);

            var state =
                new ExecuteInOtherDomainState
                {
                    Name = domainName ?? string.Concat("DynamicDomain_", Interlocked.Increment(ref _domainIndex)),
                    Permissions = permissions,
                    SecurityZone = securityZone
                };

            return ext
                .Factory
                .Replace
                .This(method)
                .Using<ExecuteInOtherDomainInterceptor<P>>(i => (Func<ExecuteInOtherDomainState, Delegate, P>) i.Execute)
                .And
                .AddProperty<ExecuteInOtherDomainState>("State", state);
        }
    }
}
