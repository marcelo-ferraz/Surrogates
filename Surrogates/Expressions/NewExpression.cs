
using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Surrogates.Expressions
{
    public class NewExpression
    {
        public ExpressionFactory<T> From<T>(string name = "")
        {
            return new ExpressionFactory<T>();
        }
    }

    public class ExpressionFactory<TBase> : Expression<TInstance>
    {


        public ReplaceExpression<TBase> Replace 
        {
            get { return new ReplaceExpression<TBase>(Strategy); } 
        }

        public DisableExpression<TBase> Disable { get; private set; }

        public VisitExpression<TBase> Visit { get; private set; }

        public ApplyExpression<TBase> Apply { get; private set; }
    }

    public class WithExpression<TBase>
    {

    }

    public class ReplaceExpression<TBase>
        : InterferenceExpression<TBase, AccessorsInterferenceExpression, UsingInterferenceExpression<TBase>>
    {

    }

    public class VisitExpression<TBase>
        : InterferenceExpression<TBase, AccessorsInterferenceExpression, UsingInterferenceExpression<TBase>>
    {
            
    }

    public abstract class InterferenceExpression<TBase, TReturn>
        : InterferenceExpression<TBase, TReturn, TReturn>
    {  }

    public abstract class InterferenceExpression<TBase, T4Prop, T4Method> : Expression<TBase>
    {
        public T4Method This(params Func<TBase, Delegate>[] method)
        {
            return (T4Method) Activator.CreateInstance(typeof(T4Method), Strategy);
        }

        public T4Prop This<T>(params Func<TBase, T>[] prop)
        {
            return (T4Prop) Activator.CreateInstance(typeof(T4Prop), Strategy);
        }
    }

    public class AccessorsInterferenceExpression<TBase>: Expression<TBase, Strategy>
    {
        public AndExpression<TBase> Accessors(Action<AccessorsModifierExpression> modExpr)
        {
            var expression = new AccessorsModifierExpression(
                new Strategy.ForProperties(CurrentStrategy));

            modExpr(expression);

            return new AndExpression<TBase>();
        }
    }

    public class AccessorsModifierExpression
    {        
        private Strategy.ForProperties _strategy;
        
        public AccessorsModifierExpression(Strategy.ForProperties strategy)
        {
            this._strategy = strategy;
        }
        
        public AccessorWithExpression Getter
        {
            get 
            {
                return new AccessorWithExpression(_strategy, PropertyAccessor.Get);
            }
        }

        public AccessorWithExpression Setter
        {
            get
            {
                return new AccessorWithExpression(_strategy, PropertyAccessor.Set);
            }
        }
    }

    public enum PropertyAccessor : byte
    {
        None = 0,
        Set = 1,
        Get = 2
    }

    public class AccessorAndExpression
    {
        private Strategy.ForProperties _strategy;
        public AccessorAndExpression(Strategy.ForProperties strategy)
        {
            _strategy = strategy;
        }

        public AccessorsModifierExpression And
        {
            get 
            {
                return new AccessorsModifierExpression(_strategy);
            }
        }
    }

    public class AccessorWithExpression
    {
        private PropertyAccessor _caller;
        private Strategy.ForProperties _strategy;
        internal MethodInfo Method { get; set; }

        public AccessorWithExpression(Strategy.ForProperties strategy, PropertyAccessor caller)
        {
            this._strategy = strategy;            
            this._caller = caller;
        }

        public AccessorAndExpression With<T>(Func<T, Delegate> interceptor)
        {            
            var holder = (T) FormatterServices
                .GetSafeUninitializedObject(typeof(T));

            if (_caller == PropertyAccessor.Set)
            {
                if (_strategy.Setter != null)
                { throw new AccessorAlreadyOverridenException("Setter"); }

                _strategy.Setter = interceptor(holder).Method; 
            }

            if (_caller == PropertyAccessor.Get)
            {
                if (_strategy.Getter != null)
                { throw new AccessorAlreadyOverridenException("Getter"); }
                
                _strategy.Getter = interceptor(holder).Method;
            }

            return new AccessorAndExpression(_strategy);
        }
    }

    public class UsingInterferenceExpression<TBase>
    {
        public MethodInterferenceExpression<TBase> Using<T>(Func<TBase, Delegate> method)
        {
            return new MethodInterferenceExpression<TBase>();
        }
    }

    public class MethodInterferenceExpression<TBase>
    {
        public AndExpression<TBase> This(Func<TBase, Delegate> method)
        {
            return new AndExpression<TBase>();
        }
    }

    public class DisableExpression<TBase>
        : InterferenceExpression<TBase, ExpressionFactory<TBase>>
    {

    }

    public class AndExpression<TBase>
    {
        public ExpressionFactory<TBase> And { get; private set; }
    }

    public class ApplyExpression<TBase>
    { 

    }

    public abstract class Expression<TInstance, TExpr>
    {
        protected TInstance NotInitializedInstance;
        protected TExpr CurrentStrategy;
        protected Strategies Strategies;

        protected Expression(Strategies strategies, TExpr strategy)
        {
            this.CurrentStrategy = strategy;            
            this.NotInitializedInstance = (TInstance)
                FormatterServices.GetSafeUninitializedObject(typeof(TInstance));
        }
    }
}
