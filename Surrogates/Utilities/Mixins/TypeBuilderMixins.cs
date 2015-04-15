using Surrogates.Model.Collections;
using Surrogates.Tactics;
using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Surrogates.Utilities.Mixins
{
    internal static class TypeBuilderMixins
    {
        private static bool Has(this MethodAttributes attrs, MethodAttributes other)
        {
            return (attrs | other) != other;
        }

        internal static void CreateConstructor(this Strategies strats)
        {
            Action<Type[], MethodAttributes> define4 =
                (types, attr) =>
                    strats.Builder.DefineConstructor(attr, CallingConventions.Standard, types)
                        .GetILGenerator()
                        .EmitConstructor(strats, types);

            var ctrs = strats.BaseType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                //.Concat(baseType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic));

            bool hasParameterlessCtr = false;

            foreach (var ctr in ctrs)
            {
                var pTypes = 
                    ctr.GetParameters().Select(p => p.ParameterType).ToArray();

                if (pTypes.Length < 1) 
                { hasParameterlessCtr = true; }

                var attrs = ctr.Attributes.Has(MethodAttributes.Public) ? 
                    MethodAttributes.Public:  
                    MethodAttributes.FamANDAssem;

                define4(pTypes, attrs);
            }

            if (!hasParameterlessCtr)
            { define4(Type.EmptyTypes, MethodAttributes.Public); }
        }
        
     
        internal static FieldBuilder DefineFieldFromProperty(this TypeBuilder builder, PropertyInfo prop)
        {
            var fieldName = prop.Name;

            fieldName = string.Concat(
                '_', Char.ToLower(fieldName[0]), fieldName.Substring(1));

            return builder.DefineField(fieldName, prop.PropertyType, FieldAttributes.Private);
        }

        internal static PropertyBuilder DefineNewProperty(this TypeBuilder builder, Type type, string name, object defaultValue=null)
        {
            var getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            #region get_StateBag

            Func<FieldBuilder, MethodBuilder> get_Prop =
                f =>
                {
                    // Define the "get" accessor method for CustomerName.
                    MethodBuilder getterBuilder = builder.DefineMethod(
                        string.Concat("get_", name), getSetAttr, type, Type.EmptyTypes);

                    ILGenerator getIL =
                        getterBuilder.GetILGenerator();

                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, f);
                    getIL.Emit(OpCodes.Ret);

                    return getterBuilder;
                };

            #endregion

            #region set_StateBag

            Func<FieldBuilder, MethodBuilder> set_StateBag =
                f =>
                {
                    // Define the "set" accessor method for CustomerName.
                    MethodBuilder setterBuilder = builder.DefineMethod(
                        string.Concat("set_", name), getSetAttr, null, new Type[] { type });

                    ILGenerator setIL =
                        setterBuilder.GetILGenerator();

                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    setIL.Emit(OpCodes.Stfld, f);
                    setIL.Emit(OpCodes.Ret);

                    return setterBuilder;
                };

            #endregion

            FieldBuilder field = builder.DefineField(
                string.Format("_{0}{1}", name.Substring(0, 1).ToLower(), name.Substring(1)), type, FieldAttributes.Private);

            var propBldr = builder.DefineProperty(
                name, PropertyAttributes.HasDefault, type, null);

            if (type.IsAssignableFrom(typeof(IDynamicMetaObjectProvider)))
            {
                var dynamicAttrCtor = typeof(DynamicAttribute)
                    .GetConstructor(Type.EmptyTypes);

                field.SetCustomAttribute(
                        new CustomAttributeBuilder(dynamicAttrCtor, new object[] { }));

                propBldr.SetCustomAttribute(
                    new CustomAttributeBuilder(dynamicAttrCtor, new object[] { }));
            }

            propBldr.SetGetMethod(get_Prop(field));
            propBldr.SetSetMethod(set_StateBag(field));
            
            return propBldr;
        }
    }
}