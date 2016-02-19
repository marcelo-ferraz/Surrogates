
using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Utilities;
using System;
using System.Security;
using System.Threading;
using Surrogates.Aspects.Utilities;
using Surrogates.Aspects.Model;
using System.Collections.Generic;
using  System.Threading.Tasks;

namespace Surrogates.Aspects.ExecutingElsewhere
{
    using otherThreadSig = Func<Delegate, object[], Dictionary<IntPtr, bool>, Dictionary<IntPtr, Task<object>>, object>;

    public class ElsewhereExpression<T>
    {
        private static long _domainIndex = 0;
        private UsingInterferenceExpression<T> _previous;
        private ShallowExtension<T> _ext;

        public ElsewhereExpression(ShallowExtension<T> ext, UsingInterferenceExpression<T> usingInterferenceExpression)
        {
            this._previous = usingInterferenceExpression;
            this._ext = ext;
        }

        /// <summary>
        /// Every call will be send to another thread, and executed there. 
        /// </summary>
        /// <param name="andForget">About the send and forget methodology. if true, it starts the new thread and dont bother waiting for it to finish. The return of the prop will be the default of the type</param>
        /// <returns></returns>
        public AndExpression<T> InOtherThread(bool andForget = false)
        {
            var expr = _previous
                .Using<ExecuteInOtherThreadInterceptor>(i => (otherThreadSig)i.Execute);

            var forgetProp =
                _ext.Strategies.MergeProperty<bool>("Forget", m => andForget);

            return expr
                .And
                .AddProperty<Dictionary<IntPtr, bool>>("Forget", forgetProp)
                .And
                .AddProperty<Dictionary<IntPtr, Task<object>>>("Tasks")
                .And
                .AddInterface<IHasTasks>();
        }
                
        public AndExpression<T> InOtherDomain(string domainName = null, SecurityZone securityZone = SecurityZone.MyComputer, params IPermission[] permissions)
        {
            if (!typeof(T).IsDefined(typeof(SerializableAttribute), true))
            { throw new ArgumentException("The surrogated type must be marked as Serializable"); }

            foreach (var m in InternalsInspector
                .Current<Strategy.ForMethods>(_previous).Methods)
            {
                if (!m.IsPublic) 
                {                    
                    throw new IncoherenceException(
                        "The method '{0}' must be public, to be sent to another domain.", m.Name);
                }
            }
            
            var state =
                new ExecuteInOtherDomain.State
                {
                    Name = domainName ?? string.Concat("DynamicDomain_", Interlocked.Increment(ref _domainIndex)),
                    Permissions = permissions,
                    SecurityZone = securityZone
                };

            return _previous
                .Using<ExecuteInOtherDomain.Interceptor>(i => (Func<ExecuteInOtherDomain.State, Delegate, object[], object>) i.Execute)
                .And
                .AddAttribute<SerializableAttribute>()
                .And
                .AddProperty<ExecuteInOtherDomain.State>("State", state);
        }
    }
}
