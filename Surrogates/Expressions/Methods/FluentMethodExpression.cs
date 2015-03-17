using Surrogates.Mappers.Entities;
using System;
using System.Reflection;

namespace Surrogates.Expressions.Methods
{
    public abstract class FluentMethodExpression<TExpression, TBase, TInstance> 
        : FluentExpression<TExpression, TBase, TInstance>
        where TExpression : Expression<TBase, TInstance>
    {
        internal FluentMethodExpression(
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
        public virtual TExpression AllPublic(Func<MethodInfo, bool> predicate)
        {
            return Expose(BindingFlags.Public, predicate);
        }

        /// <summary>
        /// Exposes all protected methods
        /// </summary>
        /// <returns></returns>
        public virtual TExpression AllProtected(Func<MethodInfo, bool> predicate)
        {
            return Expose(
                BindingFlags.NonPublic,
                m => m.IsFamily && !m.IsPrivate && predicate(m));
        }

        /// <summary>
        /// Exposes all internal methods
        /// </summary>
        /// <returns></returns>
        public virtual TExpression AllInternal(Func<MethodInfo, bool> predicate)
        {
            return Expose(
                BindingFlags.NonPublic,
                m => !m.IsFamily && !m.IsPrivate && predicate(m));
        }

        /// <summary>
        /// Exposes all internal methods
        /// </summary>
        /// <returns></returns>
        public virtual TExpression Where(Func<MethodInfo, bool> predicate)
        {
            return Expose(
                BindingFlags.Public | BindingFlags.NonPublic,
                m => !m.IsFamily && !m.IsPrivate && predicate(m));
        }
    }
}
