using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Mappers.Entities;
using Surrogates.Utilities.Mixins;

namespace Surrogates.Expressions.Methods
{
    /// <summary>
    /// Expression used to replace one or a set of methods
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    /// <typeparam name="TSubstitutor"></typeparam>
    public class MethodReplaceExpression<TBase, TSubstitutor>
        : EndExpression<TBase, TSubstitutor>
    {
        internal MethodReplaceExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state) { }

        private void ReplaceAction(MethodInfo action)
        {
            foreach (var baseMethod in State.Methods)
            {
                LocalBuilder baseMethodReturn = null;

                var gen = State.TypeBuilder.EmitOverride<TBase>(
                    action, baseMethod, GetField4<TSubstitutor>(), out baseMethodReturn);

                if (baseMethodReturn != null)
                { gen.EmitDefaultValue(baseMethod.ReturnType, baseMethodReturn); }

                gen.Emit(OpCodes.Ret);
            }
            State.Methods.Clear();
        }

        private void ReplaceFunction(MethodInfo substituteMethod)
        {
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

        protected override void Register(MethodInfo method)
        {
            if (method.ReturnType == typeof(void))
            { ReplaceAction(method); }
            else { ReplaceFunction(method); }
        }

        protected override void RegisterAction(Func<TSubstitutor, Delegate> action)
        {
            ReplaceAction(action(NotInitializedInstance).Method);
        }

        protected override void RegisterFunction(Func<TSubstitutor, Delegate> function)
        {
            // if the method of substitution returns the same type, or that type is assinable from, return that
            // all the rules from the void method are appliable here
            ReplaceFunction(function(NotInitializedInstance).Method);
        }
    }
}