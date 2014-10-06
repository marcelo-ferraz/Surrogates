using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Methods
{
    public class MethodVisitationExpression<TBase, TVisitor> 
        : MethodReplaceExpression<TBase, TVisitor>
    {
        internal MethodVisitationExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state) { }

        protected override void RegisterAction(Func<TVisitor, Delegate> action)
        {
            MethodInfo visitorMethod =
                action(NotInitializedInstance).Method;

            foreach (var baseMethod in State.Methods)
            {

                LocalBuilder baseMethodReturn = null;

                var gen = State.TypeBuilder.EmitOverride<TBase>(
                    visitorMethod, baseMethod, GetField4<TVisitor>(), out baseMethodReturn);

                gen.Emit(OpCodes.Ldarg_0);
                
                var @params =
                    gen.EmitParameters4<TBase>(baseMethod, baseMethod);

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

                var gen = State.TypeBuilder.EmitOverride<TBase>(
                    substituteMethod, baseMethod, GetField4<TVisitor>(), out baseMethodReturn);

                gen.Emit(OpCodes.Pop); 
                
                gen.Emit(OpCodes.Ldarg_0);
                
                var @params =
                    gen.EmitParameters4<TBase>(baseMethod, baseMethod);

                gen.Emit(OpCodes.Call, baseMethod);

                gen.Emit(OpCodes.Ret);
            }
            State.Methods.Clear();
        }
    }
}
