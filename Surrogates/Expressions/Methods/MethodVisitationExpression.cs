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

                LocalBuilder baseMethodReturn = null;

                var gen = State.TypeBuilder.EmitOverride(
                    visitorMethod, baseMethod, GetInterceptorField<TVisitor>(), out baseMethodReturn);

                gen.Emit(OpCodes.Ldarg_0);
                
                var @params =
                    gen.EmitParameters(baseMethod, baseMethod);

                gen.Emit(OpCodes.Call, baseMethod);
                gen.Emit(OpCodes.Ret);
            }
            State.Methods.Clear();
        }

        protected override void RegisterFunction(Func<TVisitor, Delegate> function)
        {
            MethodInfo substituteMethod =
                function(NotInitializedInstance).Method;

            foreach (var baseMethod in State.Methods)
            {
                LocalBuilder baseMethodReturn = null;

                var gen = State.TypeBuilder.EmitOverride(
                    substituteMethod, baseMethod, GetInterceptorField<TVisitor>(), out baseMethodReturn);

                gen.Emit(OpCodes.Pop); 
                
                gen.Emit(OpCodes.Ldarg_0);
                
                var @params =
                    gen.EmitParameters(baseMethod, baseMethod);

                gen.Emit(OpCodes.Call, baseMethod);

                gen.Emit(OpCodes.Ret);
            }
            State.Methods.Clear();
        }
    }
}
