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

        public UsingExpression(Strategy.ForProperties strategy, PropertyAccessor caller)
        {
            this._strategy = strategy;
            this._caller = caller;
        }

        public Accessors.AndExpression Using<T>(Func<T, Delegate> interceptor)
        {
            return Using<T>(null, interceptor);
        }

        public Accessors.AndExpression Using<T>(string name, string interceptor)
        {
            return Using<T>(name, typeof(T).GetMethod4Surrogacy(interceptor));
        }

        public Accessors.AndExpression Using<T>(string interceptor)
        {
            return Using<T>(null, typeof(T).GetMethod4Surrogacy(interceptor));
        }


        public Accessors.AndExpression Using<T>(string name, Func<T, Delegate> interceptor)
        {
            var holder = (T) FormatterServices
                .GetSafeUninitializedObject(typeof(T));

            return Using<T>(name, interceptor(holder).Method);
        }

        private AndExpression Using<T>(string name, MethodInfo method)
        {
            var @int =
              new Strategy.Interceptor(name, typeof(T), method);

            if (_caller == PropertyAccessor.Set)
            {
                if (_strategy.Setter != null)
                { throw new AccessorAlreadyOverridenException("Setter"); }

                _strategy.Setter = @int;
            }
            else if (_caller == PropertyAccessor.Get)
            {
                if (_strategy.Getter != null)
                { throw new AccessorAlreadyOverridenException("Getter"); }

                _strategy.Getter = @int;
            }

            return new Accessors.AndExpression(_strategy);
        }
    }
}