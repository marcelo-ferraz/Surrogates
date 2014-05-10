using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Expressions.Methods
{
    public class FluentExpression<TExpression, TBase, TInstance> : Expression<TBase, TInstance>
        where TExpression : FluentExpression<TExpression, TBase, TInstance>
    {
        internal FluentExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { }

        protected virtual TExpression Return()
        {
            return (TExpression)this;
        }

        protected override void RegisterAction(Func<TInstance, Delegate> action)
        {
            RegisterMethod(action(NotInitializedInstance).Method);
        }

        protected override void RegisterFunction(Func<TInstance, Delegate> function)
        {
            RegisterMethod(function(NotInitializedInstance).Method);
        }

        protected virtual void RegisterMethod(MethodInfo method)
        {
            State.Methods.Add(method);
        }

        public virtual TExpression This(Func<TInstance, Action> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0>(Func<TInstance, Action<T0>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1>(Func<TInstance, Action<T0, T1>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2>(Func<TInstance, Action<T0, T1, T2>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3>(Func<TInstance, Action<T0, T1, T2, T3>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4>(Func<TInstance, Action<T0, T1, T2, T3, T4>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6, T7>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression This<R>(Func<TInstance, Func<R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, R>(Func<TInstance, Func<T0, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, R>(Func<TInstance, Func<T0, T1, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, R>(Func<TInstance, Func<T0, T1, T2, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, R>(Func<TInstance, Func<T0, T1, T2, T3, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6, T7, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression This<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

    }
}