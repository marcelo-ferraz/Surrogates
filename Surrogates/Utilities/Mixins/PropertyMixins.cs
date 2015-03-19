using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Utilities.Mixins
{
    public static class PropertyMixins
    {
        public static bool EmitPropertyNameAndField(this Property property, Type pType, ILGenerator gen, ParameterInfo p)
        {
            if (p.Name == "s_name" && p.ParameterType == typeof(string))
            {
                gen.Emit(OpCodes.Ldstr, property.Original.Name);
                return true;
            }

            if (p.Name == "s_field" && p.ParameterType == pType)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, property.Field);
                return true;
            }

            return false;
        }

        public static bool EmitPropertyNameAndFieldAndValue(this Property property, Type pType, ILGenerator gen, ParameterInfo p)
        {
            if (p.Name == "propertyName" && p.ParameterType == typeof(string))
            {
                gen.Emit(OpCodes.Ldstr, property.Original.Name);
                return true;
            }

            if (p.Name == "field" && p.ParameterType == pType)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, property.Field);
                return true;
            }

            if (p.Name == "value" && p.ParameterType.IsAssignableFrom(property.Original.PropertyType))
            {
                gen.Emit(OpCodes.Ldarg_1);
                return true;
            }

            return false;
        }

        public virtual MethodBuilder CreateGetter(this PropertyInfo prop, ref TypeBuilder builder)
        {
            return builder.DefineMethod(
                string.Concat("get_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                prop.PropertyType,
                Type.EmptyTypes);
        }

        public MethodBuilder CreateSetter(this PropertyInfo prop, ref TypeBuilder builder)
        {
            return builder.DefineMethod(
                string.Concat("set_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(void),
                new Type[] { prop.PropertyType });
        }
    }
}
