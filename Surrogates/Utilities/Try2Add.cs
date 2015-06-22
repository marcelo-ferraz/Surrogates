using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities
{
    internal static class Try2Add
    {
        internal static bool AnythingAsParameter(ILGenerator gen, Strategy strategy, Strategy.Interceptor interceptor, MethodInfo originalMethod, ParameterInfo param)
        {
            var isSpecialParam =
                param.Name[0] == 's' && param.Name[1] == '_';
            
            if (param.Is4Name())
            {
                gen.Emit(OpCodes.Ldstr, originalMethod.Name);
                return true;
            }

            // get the instance if the parameter of the interceptor is named instance
            if (strategy.Accesses.HasFlag(Access.Instance) && param.IsInstance(strategy.BaseType))
            {
                gen.Emit(OpCodes.Ldarg_0);
                return true;
            }

            if (param.IsSelfArguments())
            {
                gen.Emit(OpCodes.Ldloc, interceptor.Locals["Args"]);
                return true;
            }

            // tries to add any method as parameter - disabled, temporarily 
            if (strategy.Accesses.HasFlag(Access.AnyMethod) && param.Is4SomeMethod())
            {
                var local = 
                    interceptor.Locals[string.Concat(param.Name, "+", param.ParameterType.Name)];

                gen.Emit(OpCodes.Ldloc, local);

                return true;
            }

            // tries to add any field as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyField) &&
                Try2Add.AnyFieldAsParameter(gen, strategy.BaseType, strategy.Fields, param, param.ParameterType))
            { return true; }

            // tries to add any property as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyBaseProperty) &&
                Try2Add.AnyBasePropertyAsParameter(gen, strategy.BaseType, strategy.Fields, param, param.ParameterType))
            { return true; }
            
            // tries to add any of the new properties as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyNewProperty) &&
                Try2Add.AnyNewPropertyAsParameter(gen, strategy.BaseType, strategy.NewProperties, param, param.ParameterType))
            { return true; }

            if (param.IsSelfMethod())
            {
                gen.Emit(OpCodes.Ldloc, interceptor.Locals["S_Method"]);
                return true;
            }

            if (param.IsDynamic_())
            {                
                gen.Emit(OpCodes.Ldstr, strategy.ThisDynamic_Type.FullName);
                gen.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetType", new [] { typeof(string) }), new [] { typeof(string) });
                gen.Emit(OpCodes.Ldloc, interceptor.Locals["ThisDynamic_"]);
                gen.EmitCall(OpCodes.Call, typeof(Activator).GetMethod("CreateInstance", new[] { typeof(Type), typeof(object[]) }), new[] { typeof(Type), typeof(object[]) });
        
                return true;
            }

            gen.EmitDefaultParameterValue(param.ParameterType); 

            return false;
        }

        internal static bool AnyBasePropertyAsParameter(ILGenerator gen, Type baseType, FieldList fields, ParameterInfo param, Type pType)
        {
            var pName =
                param.Name.Substring(2);

            var prop =
                baseType.GetProperty(
                pName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (prop == null || !prop.PropertyType.IsAssignableFrom(param.ParameterType))
            { return false; }

            gen.Emit(OpCodes.Ldarg_0);
            gen.EmitCall(prop.GetGetMethod());

            return true;
        }

        internal static bool AnyFieldAsParameter(ILGenerator gen, Type baseType, FieldList fields, ParameterInfo param, Type pType)
        {
            var fName =
                param.Name.Substring(2);

            var field =
                baseType.GetField(
                fName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            if (field == null)
            { field = fields[fName]; }

            if (field == null || !field.FieldType.IsAssignableFrom(param.ParameterType))
            { return false; }

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, field);

            return true;
        }

        internal static bool AnyNewPropertyAsParameter(this ILGenerator gen, Type baseType, List<NewProperty> newProperties, ParameterInfo param, Type pType)
        {
            for (int i = 0; i < newProperties.Count; i++)
            {
                if (!newProperties[i].Type.IsAssignableFrom(param.ParameterType))
                { continue; }

                if (newProperties[i].Name != param.Name.Substring(2))
                { continue; }

                gen.Emit(OpCodes.Ldarg_0);
                gen.EmitCall(newProperties[i].GetBuilder().GetGetMethod());

                return true;
            }

            return false;
        }
    }
}