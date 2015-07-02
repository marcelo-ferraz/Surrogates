using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Surrogates.Expressions.Accessors
{
    public class UsingExpression
    {
        private PropertyAccessor _caller;
        internal Strategy.ForProperties _strategy;
        internal MethodInfo Method { get; set; }

        private AndExpression Using(Type type, string name, MethodInfo method)
        {
            var @int =
              new Strategy.InterceptorInfo(name, type, method);

            if (_caller == PropertyAccessor.Set)
            {
                if (_strategy.Setter != null)
                { throw new AccessorAlreadyOverridenException("Setter"); }

                _strategy.Setter = @int;
                _strategy.BaseMethods.Add(method, _strategy);
            }
            else if (_caller == PropertyAccessor.Get)
            {
                if (_strategy.Getter != null)
                { throw new AccessorAlreadyOverridenException("Getter"); }

                _strategy.Getter = @int;
                _strategy.BaseMethods.Add(method, _strategy);
            }

            return new Accessors.AndExpression(_strategy);
        }

        public UsingExpression(Strategy.ForProperties strategy, PropertyAccessor caller)
        {
            this._strategy = strategy;
            this._caller = caller;
        }
        
        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T">The type of the interceptor</typeparam>
        /// <param name="name">The name of the new property which holds this interceptor</param>
        /// <param name="interceptor">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public Accessors.AndExpression Using<T>(string name, Func<T, Delegate> interceptor)
        {
            var holder = (T)FormatterServices
                .GetSafeUninitializedObject(typeof(T));

            return Using(typeof(T), name, interceptor(holder).Method);
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T">The type of the interceptor</typeparam>
        /// <param name="interceptor">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public Accessors.AndExpression Using<T>(Func<T, Delegate> interceptor)
        {
            return Using<T>(null, interceptor);
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T">The type of the interceptor</typeparam>
        /// <param name="name">The name of the new property which holds this interceptor</param>
        /// <param name="interceptor">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public Accessors.AndExpression Using<T>(string name, string interceptor)
        {
            return Using(typeof(T), name, typeof(T).GetMethod4Surrogacy(interceptor));
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T">The type of the interceptor</typeparam>
        /// <param name="interceptor">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public Accessors.AndExpression Using<T>(string interceptor)
        {
            return Using(typeof(T), null, typeof(T).GetMethod4Surrogacy(interceptor));
        }
        
        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T">The type of the interceptor</typeparam>
        /// <param name="name">The name of the new property which holds this interceptor</param>
        /// <param name="interceptor">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public Accessors.AndExpression Using(Type type, string name, string interceptor)
        {
            return Using(type, name, type.GetMethod4Surrogacy(interceptor));
        }

        /// <summary>
        /// Adds a method that will intercept the original behavior to the going expression
        /// </summary>
        /// <typeparam name="T">The type of the interceptor</typeparam>
        /// <param name="interceptor">The method which will intercept the original behavior</param>
        /// <returns></returns>
        public Accessors.AndExpression Using(Type type, string interceptor)
        {
            return Using(type, null, type.GetMethod4Surrogacy(interceptor));
        }

        public AndExpression Using(Type type, string name, string methodName, Type[] typeParameters, params Type[] parameterTypes)
        {
            var mt = type.GetMethod4Surrogacy(methodName, parameterTypes);

            if (mt.IsGenericMethodDefinition)
            { mt = mt.MakeGenericMethod(typeParameters); }

            return Using(type, name, mt);
        }

        public AndExpression Using<T>(string name, string methodName, Type[] typeParameters, params Type[] parameterTypes)
        {
            return this.Using(typeof(T), name, methodName, typeParameters, parameterTypes);
        }

        public AndExpression Using<T>(string methodName, Type[] typeParameters, params Type[] parameterTypes)
        {
            return this.Using(typeof(T), null, methodName, typeParameters, parameterTypes);
        }
    }
}