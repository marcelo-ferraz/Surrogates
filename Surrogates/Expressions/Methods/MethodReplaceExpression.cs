using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Methods
{
    public class MethodReplaceExpression<TBase, TSubstitutor>
        : EndExpression<TBase, TSubstitutor>
    {
        internal MethodReplaceExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state) { }

        protected override void RegisterAction(Func<TSubstitutor, Delegate> action)
        {
            MethodInfo substituteMethod =
                action(NotInitializedInstance).Method;

            foreach (var baseMethod in State.Methods)
            {
                LocalBuilder baseMethodReturn = null;

                var gen = State.TypeBuilder.EmitOverride<TBase>(
                    substituteMethod, baseMethod, GetField4<TSubstitutor>(), out baseMethodReturn);

                if (baseMethodReturn != null)
                { gen.EmitDefaultValue(baseMethod.ReturnType, baseMethodReturn); }

                gen.Emit(OpCodes.Ret);
            }
            State.Methods.Clear();
        }

        protected override void RegisterFunction(Func<TSubstitutor, Delegate> function)
        {
            // if the method of substitution returns the same type, or that type is assinable from, return that
            // all the rules from the void method are appliable here
            MethodInfo substituteMethod =
                function(NotInitializedInstance).Method;

            foreach (var baseMethod in State.Methods)
            {
                LocalBuilder baseMethodReturn = null;

                var gen = State.TypeBuilder.EmitOverride<TBase>(
                    substituteMethod, baseMethod, GetField4<TSubstitutor>(), out baseMethodReturn);

                //the base method is void, discard the value
                if (baseMethodReturn == null)
                {
                    gen.Emit(OpCodes.Pop);
                }
                else if (!substituteMethod.ReturnType.IsAssignableFrom(baseMethod.ReturnType))
                {
                    gen.EmitDefaultValue(substituteMethod.ReturnType, baseMethodReturn);
                }

                gen.Emit(OpCodes.Ret);
            }
            State.Methods.Clear();
        }
    }
}