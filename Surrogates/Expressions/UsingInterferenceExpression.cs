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

        public AndExpression<TBase> Using(Delegate @delegate)
        {
            Strategies.BaseMethods.Add(@delegate.Method, CurrentStrategy);

            CurrentStrategy.Interceptor =
               new Strategy.InterceptorInfo(@delegate.Method);

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(Container, new Strategy(Strategies), Strategies);
        }

        public AndExpression<TBase> Using(Type type, string name, MethodInfo method)
        {
            if (method.IsGenericMethodDefinition)
            { 
                throw new NotSupportedException("To use a generic method, you must provide its type parameters"); 
            }

            Strategies.BaseMethods.Add(method, CurrentStrategy);

            CurrentStrategy.Interceptor =
               new Strategy.InterceptorInfo(name, type, method);

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(Container, new Strategy(Strategies), Strategies);
        }

        public AndExpression<TBase> Using(Type type, string name, string methodName, Type[] typeParameters, params Type[] parameterTypes)
        {
            var mt = type.GetMethod4Surrogacy(methodName, parameterTypes);

            if (mt.IsGenericMethodDefinition)
            { mt = mt.MakeGenericMethod(typeParameters); }

            return Using(type, name, mt);
        }

        public AndExpression<TBase> Using<T>(string name, string methodName, Type[] typeParameters, params Type[] parameterTypes)
        {
            return this.Using(typeof(T), name, methodName, typeParameters, parameterTypes);
        }

        public AndExpression<TBase> Using<T>(string methodName, Type[] typeParameters, params Type[] parameterTypes)
        {
            return this.Using<T>(null, methodName, typeParameters, parameterTypes);
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName">The method which will intercept the original behavior</param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public AndExpression<TBase> Using<T>(string methodName, params Type[] parameterTypes)
        {
            return this.Using<T>(null, methodName, null, parameterTypes);
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Interceptor's property's name</param>
        /// <param name="methodName">The method which will intercept the original behavior</param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public AndExpression<TBase> Using<T>(string name, string methodName, params Type[] parameterTypes)
        {
            return Using(typeof(T), name, typeof(T).GetMethod4Surrogacy(methodName, parameterTypes));
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
        /// <param name="name">Interceptor's property's name</param>
        /// <param name="method">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public AndExpression<TBase> Using<T>(string name, Func<T, Delegate> method)
        {
            return Using(typeof(T), name, method(GetNotInit<T>()).Method);
        }
    }
}
