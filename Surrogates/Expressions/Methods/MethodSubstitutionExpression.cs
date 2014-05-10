using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Expressions.Methods
{
    public class MethodSubstitutionExpression<TBase, TSubstitutor> 
        : VoidExpression<TBase, TSubstitutor>
    {
        protected FieldBuilder SubstituteField { get; set; }

        internal MethodSubstitutionExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state){ }

        protected virtual FieldInfo GetInterceptorField<TSubstitute>()
        {
            if (SubstituteField == null)
            {
                ushort fieldCount = 0;

                for (fieldCount = 0; fieldCount < State.Fields.Count; fieldCount++)
                {
                    if (State.Fields[fieldCount].FieldType == typeof(TSubstitute))
                    { return State.Fields[fieldCount]; }
                }

                string name = string.Concat(
                    "_interceptor", fieldCount.ToString());

                SubstituteField = State.TypeBuilder
                    .DefineField(name, typeof(TSubstitute), FieldAttributes.Private);

                State.Fields.Add(SubstituteField);
            }

            return SubstituteField;
        }

        protected override void RegisterAction(Func<TSubstitutor, Delegate> action)
        {
            MethodInfo substituteMethod = 
                action(NotInitializedInstance).Method;

            foreach (var baseMethod in State.Methods)
            {
                State.TypeBuilder.EmitOverride(
                    substituteMethod, baseMethod, GetInterceptorField<TSubstitutor>());
            }
            State.Fields.Clear();
        }

        protected override void RegisterFunction(Func<TSubstitutor, Delegate> function)
        {
            // if the method of substitution returns the same type, or that type is assinable from, return that
            // all the rules from the void method are appliable here
            MethodInfo substituteMethod =
                function(NotInitializedInstance).Method;

            foreach (var baseMethod in State.Methods)
            {
                var gen = State.TypeBuilder.EmitOverride(
                    substituteMethod, baseMethod, GetInterceptorField<TSubstitutor>());

                if (!substituteMethod.ReturnType.IsAssignableFrom(baseMethod.ReturnType))
                {                
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.EmitDefaultValue(substituteMethod.ReturnType); 
                }

                gen.Emit(OpCodes.Ret);
            }
            State.Fields.Clear();
        }
    }
}