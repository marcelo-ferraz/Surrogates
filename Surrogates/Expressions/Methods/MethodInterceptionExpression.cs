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
    public class MethodInterceptionExpression<TBase>
        : FluentExpression<MethodInterceptionExpression<TBase>, TBase, TBase>
    {
        protected TypeBuilder Typebuilder;
        protected InterceptionKind Kind;

        internal MethodInterceptionExpression(
            IMappingExpression<TBase> mapper, MappingState state, InterceptionKind kind)
            : base(mapper, state)
        {
            Kind = kind;
        }

        public IMappingExpression<TBase> And { get { return Mapper; } }

        public virtual MethodInterceptionExpression<TBase> AllPublic()
        {
            var @public = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < @public.Length; i++)
            {
                RegisterMethod(@public[i]);
            }
            return this;
        }

        public virtual MethodInterceptionExpression<TBase> AllProtectedMethods()
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].IsFamily && !methods[i].IsPrivate)
                {
                    RegisterMethod(methods[i]);
                }
            }
            return this;
        }

        public virtual MethodInterceptionExpression<TBase> AllInternalMethods()
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; i++)
            {
                if (!methods[i].IsFamily && !methods[i].IsPrivate)
                {
                    RegisterMethod(methods[i]);
                }
            }
            return this;
        }

        public virtual MethodInterceptionExpression<TBase> Where(Func<MethodInfo, bool> predicate)
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < methods.Length; i++)
            {
                if ((methods[i].IsFamily || methods[i].IsPublic) &&
                    (predicate(methods[i])))
                {
                    RegisterMethod(methods[i]);
                }
            }

            return this;
        }

        public virtual VoidExpression<TBase, TInterceptor> With<TInterceptor>()
        {
            return 
                this.Kind == InterceptionKind.Substitution ?
                (VoidExpression<TBase, TInterceptor>)new MethodSubstitutionExpression<TBase, TInterceptor>(Mapper, State) :
                new MethodVisitationExpression<TBase, TInterceptor>(Mapper, State);
        }
    }
}
