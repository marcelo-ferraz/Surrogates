using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using Surrogates.SDILReader;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Expressions
{
    public class FluentExpression<TExpression, TBase, TInstance> : Expression<TBase, TInstance>
        where TExpression : Expression<TBase, TInstance>
    {
        internal FluentExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { }

        protected virtual TExpression Return()
        {
            return this as TExpression;
        }

        protected virtual void RegisterAction(Func<TInstance, Delegate> action)
        {
            Register(action(NotInitializedInstance).Method);
        }

        protected virtual void RegisterFunction(Func<TInstance, Delegate> function)
        {
            Register(function(NotInitializedInstance).Method);
        }

        protected virtual void Register(MethodInfo method)
        {
            State.Methods.Add(method);
        }

        public virtual TExpression ThisMethod(Func<TInstance, Action> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0>(Func<TInstance, Action<T0>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1>(Func<TInstance, Action<T0, T1>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2>(Func<TInstance, Action<T0, T1, T2>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3>(Func<TInstance, Action<T0, T1, T2, T3>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4>(Func<TInstance, Action<T0, T1, T2, T3, T4>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        public virtual TExpression ThisMethod<R>(Func<TInstance, Func<R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, R>(Func<TInstance, Func<T0, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, R>(Func<TInstance, Func<T0, T1, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, R>(Func<TInstance, Func<T0, T1, T2, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, R>(Func<TInstance, Func<T0, T1, T2, T3, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }
    }
}