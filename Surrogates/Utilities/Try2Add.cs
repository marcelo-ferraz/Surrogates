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
                if (!param.ParameterType.IsAssignableFrom(newProperties[i].Type))
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