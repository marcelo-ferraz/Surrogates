﻿using Surrogates.Model;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities.Mixins
{
    public static class PropertyMixins
    {
        public static bool EmitPropertyNameAndFieldAndValue(this SurrogatedProperty property, ILGenerator gen, ParameterInfo p, int pIndex)
        {
            if (p.Name == "s_name" && p.ParameterType == TypeOf.String)
            {
                gen.Emit(OpCodes.Ldstr, property.Original.Name);
                return true;
            }

            if (p.Name == "s_field" && p.ParameterType.IsAssignableFrom(p.ParameterType))
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, property.Field);

                return true;
            }
            if (p.Name == "s_value" && p.ParameterType.IsAssignableFrom(property.Original.PropertyType))
            {
                gen.EmitArg(1);
                return true;
            }

            return false;
        }

        public static MethodBuilder CreateGetter(this PropertyInfo prop, ref TypeBuilder builder)
        {
            return builder.DefineMethod(
                string.Concat("get_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                prop.PropertyType,
                Type.EmptyTypes);
        }

        public static MethodBuilder CreateSetter(this PropertyInfo prop, ref TypeBuilder builder)
        {
            return builder.DefineMethod(
                string.Concat("set_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                TypeOf.Void,
                new Type[] { prop.PropertyType });
        }
    }
}
