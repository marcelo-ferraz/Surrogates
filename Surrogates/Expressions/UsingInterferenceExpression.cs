using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Reflection;

namespace Surrogates.Expressions
{
    public class UsingInterferenceExpression<TBase> : Expression<TBase, Strategy.ForMethods>
    {
        public UsingInterferenceExpression(BaseContainer4Surrogacy container, Strategy.ForMethods current, Strategies strategies)
            : base(container, current, strategies) { }

        private AndExpression<TBase> Using<T>(string name, MethodInfo method)
        {
            Strategies.BaseMethods.Add(method, CurrentStrategy);

            CurrentStrategy.Interceptor =
               new Strategy.Interceptor(name, typeof(T), method);

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(Container, new Strategy(Strategies), Strategies);
        }

        public AndExpression<TBase> Using<T>(string method, params Type[] parameterTypes)
        {
            return this.Using<T>(null, method, parameterTypes);
        }

        public AndExpression<TBase> Using<T>(string name, string method, params Type[] parameterTypes)
        {
            return Using<T>(name, typeof(T).GetMethod4Surrogacy(method, parameterTypes));
        }

        public AndExpression<TBase> Using<T>(Func<T, Delegate> method)
        {
            return Using<T>(null, method);
        }

        public AndExpression<TBase> Using<T>(string name, Func<T, Delegate> method)
        {
            return Using<T>(name, method(GetNotInit<T>()).Method);
        }
    }
}
