
using Surrogates.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using State = Surrogates.Applications.ExecutingElsewhere.ExecuteInOtherDomainInterceptor.State;

namespace Surrogates.Applications.ExecutingElsewhere
{
    public class ElsewhereExpression<T>
    {
        private UsingInterferenceExpression<T> _previousExpression;

        public ElsewhereExpression(UsingInterferenceExpression<T> usingInterferenceExpression)
        {
            this._previousExpression = usingInterferenceExpression;
        }

        public AndExpression<T> InAnotherDomain(string name, SecurityZone securityZone = SecurityZone.MyComputer)
        {
            var state = new State { Name = name, SecurityZone = securityZone };

            return _previousExpression.Using<ExecuteInOtherDomainInterceptor>(
                elsewhere =>
                    (Func<State, Delegate, object>)elsewhere.Execute)
                .And
                .AddProperty<State>("State", state);
        }
    }
}
