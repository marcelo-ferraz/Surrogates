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
        internal static void AnythingElseAsParameter(ILGenerator gen, Strategy strategy, Strategy.Interceptor interceptor, MethodInfo originalMethod, ParameterInfo param)
        {
            var isSpecialParam =
                param.Name[0] == 's' && param.Name[1] == '_';

            if (param.Is4Name())
            {
                gen.Emit(OpCodes.Ldstr, originalMethod.Name);
                return;
            }

            // get the instance if the parameter of the interceptor is named instance
            if (strategy.Accesses.HasFlag(Access.Instance) && param.IsInstance(strategy.BaseType))
            {
                gen.Emit(OpCodes.Ldarg_0);
                return;
            }

            if (param.IsSelfArguments())
            {
                gen.Emit(OpCodes.Ldloc, interceptor.Locals["Args"]);
                return;
            }

            // tries to add any method as parameter - disabled, temporarily 
            if (strategy.Accesses.HasFlag(Access.AnyMethod) && param.Is4SomeMethod())
            {
                var local =
                    interceptor.Locals[string.Concat(param.Name, "+", param.ParameterType.Name)];

                gen.Emit(OpCodes.Ldloc, local);

                return;
            }

            // tries to add any field as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyField) &&
                Try2Add.AnyFieldAsParameter(gen, strategy.BaseType, strategy.Fields, param, param.ParameterType))
            { return; }

            // tries to add any property as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyBaseProperty) &&
                Try2Add.AnyBasePropertyAsParameter(gen, strategy.BaseType, strategy.Fields, param, param.ParameterType))
            { return; }

            // tries to add any of the new properties as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyNewProperty) &&
                Try2Add.AnyNewPropertyAsParameter(gen, strategy.BaseType, strategy.NewProperties, param, param.ParameterType))
            { return; }

            if (param.IsSelfMethod())
            {
                gen.Emit(OpCodes.Ldloc, interceptor.Locals["S_Method"]);
                return;
            }

            if (param.IsDynamic_())
            {
                gen.Emit(OpCodes.Ldstr, strategy.ThisDynamic_Type.FullName);
                gen.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetType", new[] { typeof(string) }), new[] { typeof(string) });
                gen.Emit(OpCodes.Ldloc, interceptor.Locals["ThisDynamic_"]);
                gen.EmitCall(OpCodes.Call, typeof(Activator).GetMethod("CreateInstance", new[] { typeof(Type), typeof(object[]) }), new[] { typeof(Type), typeof(object[]) });

                return;
            }

            gen.EmitDefaultValue(param.ParameterType);
        }

    }
}
