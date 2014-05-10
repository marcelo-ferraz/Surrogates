using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Expressions.Methods
{
    public abstract class VoidExpression<TBase, TInstance> : Expression<TBase, TInstance>
    {
        internal VoidExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { }

        private AndExpression<TBase> And()
        {
            return new AndExpression<TBase>(Mapper);
        }

        public virtual AndExpression<TBase> This<T0>(Func<TInstance, Action<T0>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1>(Func<TInstance, Action<T0, T1>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2>(Func<TInstance, Action<T0, T1, T2>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3>(Func<TInstance, Action<T0, T1, T2, T3>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4>(Func<TInstance, Action<T0, T1, T2, T3, T4>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6, T7>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action)
        {
            this.RegisterAction(action);
            return And();
        }

        public virtual AndExpression<TBase> This<R>(Func<TInstance, Func<R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, R>(Func<TInstance, Func<T0, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, R>(Func<TInstance, Func<T0, T1, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, R>(Func<TInstance, Func<T0, T1, T2, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, R>(Func<TInstance, Func<T0, T1, T2, T3, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6, T7, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }

        public virtual AndExpression<TBase> This<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> function)
        {
            this.RegisterFunction(function);
            return And();
        }
    }
}