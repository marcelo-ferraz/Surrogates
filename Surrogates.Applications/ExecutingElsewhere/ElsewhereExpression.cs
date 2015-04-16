﻿
using Surrogates.Expressions;
using System;
using System.Security;
using System.Threading;

namespace Surrogates.Applications.ExecutingElsewhere
{
    public class ElsewhereExpression<T>
    {
        private static long _domainIndex = 0;
        private UsingInterferenceExpression<T> _previousExpression;

        public ElsewhereExpression(UsingInterferenceExpression<T> usingInterferenceExpression)
        {
            this._previousExpression = usingInterferenceExpression;
        }

        /// <summary>
        /// Every call will be send to another thread, and executed there. 
        /// </summary>
        /// <param name="andForget">About the send and forget methodology. if true, it starts the new thread and dont bother waiting for it to finish. The return of the method will be the default of the type</param>
        /// <returns></returns>
        public AndExpression<T> InOtherThread<P>(bool andForget = false)
        {
            return _previousExpression
                .Using<ExecuteInOtherThreadInterceptor<P>>(i => (Func<Delegate, object[], bool, P>) i.Execute)
                .And
                .AddProperty<bool>("s_Forget", andForget);
        }
                
        public AndExpression<T> InOtherDomain<P>(string domainName = null, SecurityZone securityZone = SecurityZone.MyComputer, params IPermission[] permissions)
        {
            if (!typeof(T).IsDefined(typeof(SerializableAttribute), true))
            { throw new ArgumentException("The surrogated type must be marked as Serializable"); }

            var state =
                new ExecuteInOtherDomain.State
                {
                    Name = domainName ?? string.Concat("DynamicDomain_", Interlocked.Increment(ref _domainIndex)),
                    Permissions = permissions,
                    SecurityZone = securityZone
                };

            return _previousExpression
                .Using<ExecuteInOtherDomain.Interceptor<P>>(i => (Func<ExecuteInOtherDomain.State, Delegate, P>) i.Execute)
                .And
                .AddAttribute<SerializableAttribute>()
                .And
                .AddProperty<ExecuteInOtherDomain.State>("State", state);
        }
    }
}
