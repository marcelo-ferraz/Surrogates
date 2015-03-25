using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Expressions.Accessors
{
    public class WithExpression
    {
        private PropertyAccessor _caller;
        internal Strategy.ForProperties _strategy;
        internal MethodInfo Method { get; set; }

        public WithExpression(Strategy.ForProperties strategy, PropertyAccessor caller)
        {
            this._strategy = strategy;
            this._caller = caller;
        }

        public Accessors.AndExpression With<T>(Func<T, Delegate> interceptor)
        {
            return With<T>(null, interceptor);
        }

        public Accessors.AndExpression With<T>(string name, Func<T, Delegate> interceptor)
        {
            var holder = (T)FormatterServices
                .GetSafeUninitializedObject(typeof(T));
                
            _strategy.Fields.TryAdd<T>(ref name);

            var @int =
              new Strategy.Interceptor
              {
                  DeclaredType = typeof(T),
                  Name = name,
                  Method = interceptor(holder).Method
              };

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