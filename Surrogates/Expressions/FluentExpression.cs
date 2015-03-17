using System;
using System.Collections.Generic;
using System.Reflection;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

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

        private void RegisterByName(string methodName)
        {
            Func<BindingFlags, MethodInfo> get = flags =>
                typeof(TInstance).GetMethod(methodName, BindingFlags.Instance | flags);

            MethodInfo method;

            if ((method = get(BindingFlags.NonPublic)) == null)
            {
                if ((method = get(BindingFlags.Public)) == null)
                {
                    throw new KeyNotFoundException(string.Format(
                        "The method '{0}' wans not found withn the type '{1}'", methodName, typeof(TInstance).Name));
                }
            }
            Register(method);
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

        public virtual TExpression ThisMethod(Delegate action)
        {
            this.Register(action.Method);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="methodName">The name of that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod(string methodName)
        {
            this.RegisterByName(methodName);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod(Func<TInstance, Action> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0>(Func<TInstance, Action<T0>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1>(Func<TInstance, Action<T0, T1>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2>(Func<TInstance, Action<T0, T1, T2>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3>(Func<TInstance, Action<T0, T1, T2, T3>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4>(Func<TInstance, Action<T0, T1, T2, T3, T4>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<TInstance, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action)
        {
            this.RegisterAction(action);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<R>(Func<TInstance, Func<R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, R>(Func<TInstance, Func<T0, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, R>(Func<TInstance, Func<T0, T1, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, R>(Func<TInstance, Func<T0, T1, T2, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, R>(Func<TInstance, Func<T0, T1, T2, T3, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }

        /// <summary>
        /// Exposes a given method, to become part of the expression
        /// </summary>
        /// <param name="action">The path to that method</param>
        /// <returns></returns>
        public virtual TExpression ThisMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<TInstance, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> function)
        {
            this.RegisterFunction(function);
            return Return();
        }
    }
}