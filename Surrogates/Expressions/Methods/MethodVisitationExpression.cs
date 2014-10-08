using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Methods
{
    public class MethodVisitationExpression<TBase, TVisitor> 
        : MethodReplaceExpression<TBase, TVisitor>
    {
        internal MethodVisitationExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state) { }

        [TargetedPatchingOptOut("")]
        private void VisitAction(MethodInfo action)
        {
            foreach (var baseMethod in State.Methods)
            {
                LocalBuilder baseMethodReturn = null;

                var gen = State.TypeBuilder.EmitOverride<TBase>(
                    action, baseMethod, GetField4<TVisitor>(), out baseMethodReturn);

                gen.Emit(OpCodes.Ldarg_0);

                var @params =
                    gen.EmitParameters4<TBase>(baseMethod, baseMethod);

                gen.Emit(OpCodes.Call, baseMethod);
                gen.Emit(OpCodes.Ret);
            }
            
            State.Methods.Clear();
        }

        [TargetedPatchingOptOut("")]
        private void VisitFunction(MethodInfo function)
        {
            foreach (var baseMethod in State.Methods)
            {
                LocalBuilder baseMethodReturn = null;

                var gen = State.TypeBuilder.EmitOverride<TBase>(
                    function, baseMethod, GetField4<TVisitor>(), out baseMethodReturn);

                gen.Emit(OpCodes.Pop);

                gen.Emit(OpCodes.Ldarg_0);

                var @params =
                    gen.EmitParameters4<TBase>(baseMethod, baseMethod);

                gen.Emit(OpCodes.Call, baseMethod);

                gen.Emit(OpCodes.Ret);
            }
            
            State.Methods.Clear();
        }

        protected override void Register(MethodInfo method)
        {
            if (method.ReturnType == typeof(void))
            { VisitAction(method); }
            else 
            { VisitFunction(method); }
        }

        protected override void RegisterAction(Func<TVisitor, Delegate> action)
        {
            VisitAction(action(NotInitializedInstance).Method);
        }

        protected override void RegisterFunction(Func<TVisitor, Delegate> function)
        {
            VisitFunction(function(NotInitializedInstance).Method);
        }
    }
}
