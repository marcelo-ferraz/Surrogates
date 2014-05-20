using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Properties;
using Surrogates.Mappers;
using Surrogates.SDILReader;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Expressions.Methods
{
    public class MethodSubstitutionExpression<TBase, TSubstitutor> 
        : EndExpression<TBase, TSubstitutor>
    {
        internal MethodSubstitutionExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state){ }

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

        //public PropertyReplaceExpression<TBase, T> ThisMethodProperty<T>(Func<TBase, T> propGetter)
        //{
        //    var reader =
        //        new MethodBodyReader(propGetter.Method);

        //    string propName = null;

        //    for (int i = 0; i < reader.Instructions.Count; i++)
        //    {
        //        var code =
        //            reader.Instructions[i].Code.Name;

        //        if (code != "callvirt" && code != "call")
        //        { continue; }

        //        if (!(reader.Instructions[1].Operand is MethodInfo))
        //        { continue; }

        //        propName = ((MethodInfo)reader.Instructions[i].Operand).Name;

        //        if (!propName.Contains("get_") && !propName.Contains("set_"))
        //        { throw new ArgumentException("What was provided is not an property"); }

        //        propName = propName
        //            .Replace("get_", string.Empty)
        //            .Replace("set_", string.Empty);
        //    }

        //    if (string.IsNullOrEmpty(propName))
        //    { throw new ArgumentException("What was provided is not a call for an property"); }

        //    State.Properties.Add(
        //        typeof(TBase).GetProperty(propName));

        //    return new PropertyExpression<TBase,T>(Mapper, State);
        //}
    }
}