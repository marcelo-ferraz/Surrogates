using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Methods
{
    public class MethodInterferenceExpression<TBase>
     : FluentExpression<EndMethodInterferenceExpression<TBase>, TBase, TBase>
    {
        protected TypeBuilder Typebuilder;
        protected InterferenceKind Kind;

        internal MethodInterferenceExpression(
            IMappingExpression<TBase> mapper, MappingState state, InterferenceKind kind)
            : base(mapper, state)
        {
            Kind = kind;
        }

        protected override EndMethodInterferenceExpression<TBase> Return()
        {
            return new EndMethodInterferenceExpression<TBase>(this.Mapper, this.State, Kind);
        }

        protected virtual EndMethodInterferenceExpression<TBase> Expose(
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
        public virtual EndMethodInterferenceExpression<TBase> PublicMethods(Func<MethodInfo, bool> predicate)
        {
            return Expose(BindingFlags.Public,  predicate);
        }

        /// <summary>
        /// Exposes all protected methods
        /// </summary>
        /// <returns></returns>
        public virtual EndMethodInterferenceExpression<TBase> ProtectedMethods(Func<MethodInfo, bool> predicate)
        {
            return Expose(
                BindingFlags.NonPublic,
                m => m.IsFamily && !m.IsPrivate && predicate(m));
        }

        /// <summary>
        /// Exposes all internal methods
        /// </summary>
        /// <returns></returns>
        public virtual EndMethodInterferenceExpression<TBase> InternalMethods(Func<MethodInfo, bool> predicate)
        {
            return Expose(
                BindingFlags.NonPublic,
                m => !m.IsFamily && !m.IsPrivate && predicate(m));
        }

        /// <summary>
        /// Exposes all methods that satisfy a given predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual EndMethodInterferenceExpression<TBase> Methods(Func<MethodInfo, bool> predicate)
        {
            return Expose(BindingFlags.NonPublic | BindingFlags.Public, predicate);
        }
    }
}