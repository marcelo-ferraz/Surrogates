using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities
{
    public static class JustAdd
    {
        internal static void AnythingElseAsParameter(ILGenerator gen, Strategy strategy, OverridenMethod overriden, MethodInfo originalMethod, ParameterInfo param)
        {
            if (param.Is4Name())
            {
                gen.Emit(OpCodes.Ldstr, originalMethod.Name);
                return;
            }

            // get the instance if the parameter of the interceptor is named instance
            if (strategy.Accesses.HasFlag(Access.Instance) && param.Is4Instance(strategy.BaseType))
            {
                gen.Emit(OpCodes.Ldarg_0);
                return;
            }

            if (param.Is4SelfArguments())
            {
                gen.Emit(OpCodes.Ldloc, overriden.Locals["Args"]);
                return;
            }

            // tries to add any method as parameter - disabled, temporarily 
            if (strategy.Accesses.HasFlag(Access.AnyMethod) && param.Is4SomeMethod())
            {
                var local =
                    overriden.Locals[string.Concat(param.Name, "+", param.ParameterType.Name)];

                gen.Emit(OpCodes.Ldloc, local);

                return;
            }

            // tries to add any srcField as parameter 
            if (param.Is4AnyField() &&
                strategy.Accesses.HasFlag(Access.AnyField) &&
                Try2Add.AnyFieldAsParameter(gen, strategy.BaseType, strategy.Fields, param, param.ParameterType))
            { return; }

            // tries to add any property as parameter 
            if (param.Is4anyProperty() &&
                strategy.Accesses.HasFlag(Access.AnyBaseProperty) &&
                Try2Add.AnyBasePropertyAsParameter(gen, strategy.BaseType, strategy.Fields, param, param.ParameterType))
            { return; }

            // tries to add any of the new properties as parameter 
            if (param.Is4anyProperty() &&
                strategy.Accesses.HasFlag(Access.AnyNewProperty) &&
                Try2Add.AnyNewPropertyAsParameter(gen, strategy.BaseType, strategy.NewProperties, param, param.ParameterType))
            { return; }

            if (param.Is4SelfMethod(originalMethod))
            {
                gen.Emit(OpCodes.Ldloc, overriden.Locals["S_Method"]);
                return;
            }

            if (param.IsDynamic_())
            {
                gen.Emit(OpCodes.Ldstr, strategy.ThisDynamic_Type.FullName);
                gen.EmitCall(OpCodes.Call, TypeOf.Type.GetMethod("GetType", new[] { TypeOf.String }), new[] { TypeOf.String });
                gen.Emit(OpCodes.Ldloc, overriden.Locals["ThisDynamic_"]);
                gen.EmitCall(OpCodes.Call, TypeOf.Activator.GetMethod("CreateInstance", new[] { TypeOf.Type, TypeOf.ObjectArray }), new[] { TypeOf.Type, TypeOf.ObjectArray });

                return;
            }

            gen.EmitDefaultValue(param.ParameterType);
        }

    }
}
