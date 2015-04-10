using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;

namespace Surrogates.Expressions
{
    public class UsingInterferenceExpression<TBase> : Expression<TBase, Strategy.ForMethods>
    {
        public UsingInterferenceExpression(BaseContainer4Surrogacy container, Strategy.ForMethods current, Strategies strategies)
            : base(container, current, strategies) { }

        public AndExpression<TBase> Using<T>(string method)
        {
            return this.Using<T>(null, method);
        }

        public AndExpression<TBase> Using<T>(string name, string method)
        {        
            //Strategies.Fields.TryAdd<T>(ref name);
            
            CurrentStrategy.Interceptor =
               new Strategy.Interceptor(name, typeof(T), typeof(T).GetMethod4Surrogacy(method));

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(Container, new Strategy(Strategies), Strategies);
        }

        public AndExpression<TBase> Using<T>(Func<T, Delegate> method)
        {
            return Using<T>(null, method);
        }

        public AndExpression<TBase> Using<T>(string name, Func<T, Delegate> method)
        {
            //Strategies.Fields.TryAdd<T>(ref name);
            
            CurrentStrategy.Interceptor =
               new Strategy.Interceptor(name, typeof(T), method(GetNotInit<T>()).Method);   

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(Container, new Strategy(Strategies), Strategies);
        }
    }
}
