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

        private AndExpression<TBase> Using(Type type, string name, MethodInfo method)
        {
            Strategies.BaseMethods.Add(method, CurrentStrategy);

            CurrentStrategy.Interceptor =
               new Strategy.InterceptorInfo(name, type, method);

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(Container, new Strategy(Strategies), Strategies);
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method which will intercept the original behavior</param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public AndExpression<TBase> Using<T>(string method, params Type[] parameterTypes)
        {
            return this.Using<T>(null, method, parameterTypes);
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="method">The method which will intercept the original behavior</param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public AndExpression<TBase> Using<T>(string name, string method, params Type[] parameterTypes)
        {
            return Using(typeof(T), name, typeof(T).GetMethod4Surrogacy(method, parameterTypes));
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public AndExpression<TBase> Using<T>(Func<T, Delegate> method)
        {
            return Using<T>(null, method);
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the method</param>
        /// <param name="method">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public AndExpression<TBase> Using<T>(string name, Func<T, Delegate> method)
        {
            return Using(typeof(T), name, method(GetNotInit<T>()).Method);
        }
    }
}
