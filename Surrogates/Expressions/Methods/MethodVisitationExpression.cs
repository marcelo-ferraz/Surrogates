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
    public class MethodVisitationExpression<TBase, TVisitor> 
        : MethodSubstitutionExpression<TBase, TVisitor>
    {
        internal MethodVisitationExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { }

        protected override void RegisterAction(Func<TVisitor, Delegate> action)
        {
            MethodInfo visitorMethod =
                action(NotInitializedInstance).Method;

            foreach (var baseMethod in State.Methods)
            {
                var gen = State.TypeBuilder.EmitOverride(
                    visitorMethod, baseMethod, GetInterceptorField<TVisitor>());

                gen.Emit(OpCodes.Nop);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Call, baseMethod);
            }
            State.Fields.Clear();
        }

        protected override void RegisterFunction(Func<TVisitor, Delegate> function)
        {
            MethodInfo visitorMethod =
                function(NotInitializedInstance).Method;

            foreach (var baseMethod in State.Methods)
            {
                var gen = State.TypeBuilder.EmitOverride(
                    visitorMethod, baseMethod, GetInterceptorField<TVisitor>());

                gen.Emit(OpCodes.Nop);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Call, baseMethod);

                gen.Emit(OpCodes.Ret);
            }
            State.Fields.Clear();
        }
    }
}
