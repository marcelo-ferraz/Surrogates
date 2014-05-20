using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Expressions.Methods
{
    public class MethodInterferenceExpression<TBase>
        : FluentExpression<MethodInterferenceExpression<TBase>, TBase, TBase>
    {
        protected TypeBuilder Typebuilder;
        protected InterferenceKind Kind;

        internal MethodInterferenceExpression(
            IMappingExpression<TBase> mapper, MappingState state, InterferenceKind kind)
            : base(mapper, state)
        {
            Kind = kind;
        }

        public virtual MethodInterferenceExpression<TBase> PublicMethods()
        {
            var @public = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < @public.Length; i++)
            {
                Register(@public[i]);
            }
            return this;
        }

        public virtual MethodInterferenceExpression<TBase> ProtectedMethods()
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].IsFamily && !methods[i].IsPrivate)
                {
                    Register(methods[i]);
                }
            }
            return this;
        }

        public virtual MethodInterferenceExpression<TBase> InternalMethods()
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; i++)
            {
                if (!methods[i].IsFamily && !methods[i].IsPrivate)
                {
                    Register(methods[i]);
                }
            }
            return this;
        }

        public virtual MethodInterferenceExpression<TBase> Methods(Func<MethodInfo, bool> predicate)
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < methods.Length; i++)
            {
                if ((methods[i].IsFamily || methods[i].IsPublic) &&
                    (predicate(methods[i])))
                {
                    Register(methods[i]);
                }
            }

            return this;
        }

        public virtual EndExpression<TBase, TInterceptor> Using<TInterceptor>()
        {
            return 
                this.Kind == InterferenceKind.Substitution ?
                (EndExpression<TBase, TInterceptor>)new MethodSubstitutionExpression<TBase, TInterceptor>(Mapper, State) :
                new MethodVisitationExpression<TBase, TInterceptor>(Mapper, State);
        }
    }
}
