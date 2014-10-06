using System;
using System.Reflection;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Methods
{
    public abstract class FluentExpression<TExpression, TBase, TInstance> 
        : Surrogates.Expressions.FluentExpression<TExpression, TBase, TInstance>
        where TExpression : Expression<TBase, TInstance>
    {
        internal FluentExpression(
            IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state) { }
        
        protected virtual TExpression Expose(
            BindingFlags flags, Func<MethodInfo, bool> predicate = null)
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | flags);

            for (int i = 0; i < methods.Length; i++)
            {
                if (predicate == null && predicate(methods[i]))
                {
                    Register(methods[i]);
                }
            }
            return Return();
        }

        /// <summary>
        /// Exposes all public methods
        /// </summary>
        /// <returns></returns>
        public virtual TExpression PublicMethods(Func<MethodInfo, bool> predicate)
        {
            return Expose(BindingFlags.Public, predicate);
        }

        /// <summary>
        /// Exposes all protected methods
        /// </summary>
        /// <returns></returns>
        public virtual TExpression ProtectedMethods(Func<MethodInfo, bool> predicate)
        {
            return Expose(
                BindingFlags.NonPublic,
                m => m.IsFamily && !m.IsPrivate && predicate(m));
        }

        /// <summary>
        /// Exposes all internal methods
        /// </summary>
        /// <returns></returns>
        public virtual TExpression InternalMethods(Func<MethodInfo, bool> predicate)
        {
            return Expose(
                BindingFlags.NonPublic,
                m => !m.IsFamily && !m.IsPrivate && predicate(m));
        }

        /// <summary>
        /// Exposes all internal methods
        /// </summary>
        /// <returns></returns>
        public virtual TExpression Methods(Func<MethodInfo, bool> predicate)
        {
            return Expose(
                BindingFlags.Public | BindingFlags.NonPublic,
                m => !m.IsFamily && !m.IsPrivate && predicate(m));
        }
    }
}
