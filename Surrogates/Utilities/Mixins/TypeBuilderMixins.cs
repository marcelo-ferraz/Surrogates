using Surrogates.Model.Collections;
using System;
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

        private static PropertyBuilder DefineProperty<T>(this TypeBuilder builder, string name, Func<TypeBuilder, FieldBuilder, PropertyBuilder> getProp)
        {
            var getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            #region get_StateBag

            Func<FieldBuilder, MethodBuilder> get_Prop =
                f =>
                {
                    // Define the "get" accessor method for CustomerName.
                    MethodBuilder getterBuilder = builder.DefineMethod(
                        string.Concat("get_", name), getSetAttr, typeof(T), Type.EmptyTypes);

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
                        string.Concat("set_", name), getSetAttr, null, new Type[] { typeof(T) });

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
                string.Format("_{0}{1}", name.Substring(0, 1).ToLower(), name.Substring(1)), typeof(T), FieldAttributes.Private);

            var propBldr =
                getProp(builder, field);

            propBldr.SetGetMethod(get_Prop(field));
            propBldr.SetSetMethod(set_StateBag(field));

            return propBldr;
        }
        
        internal static void CreateConstructor4<T>(this TypeBuilder typeBuilder, FieldList fields)
        {
            typeBuilder.CreateConstructor(typeof(T), fields);
        }

        internal static void CreateConstructor(this TypeBuilder typeBuilder, Type baseType, FieldList fields)
        {
            Action<Type[], MethodAttributes> define4 =
                (types, attr) =>
                    typeBuilder.DefineConstructor(attr, CallingConventions.Standard, types)
                        .GetILGenerator()
                        .EmitConstructor(baseType, fields, types);

            var ctrs = baseType
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
        
        internal static ILGenerator EmitOverride<TBase>(this TypeBuilder typeBuilder, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField, FieldList fields)
        {
            return EmitOverride(typeBuilder, typeof(TBase), newMethod, baseMethod, interceptorField, fields);
        }

        internal static ILGenerator EmitOverride(this TypeBuilder typeBuilder, Type baseType, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField, FieldList fields)
        {
            LocalBuilder @return = null;
            return EmitOverride(typeBuilder, baseType, newMethod, baseMethod, interceptorField, fields, out @return);
        }

        internal static ILGenerator EmitOverride<TBase>(this TypeBuilder typeBuilder, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField, FieldList fields, out LocalBuilder returnField)
        {
            return EmitOverride(typeBuilder, typeof(TBase), newMethod, baseMethod, interceptorField, fields, out returnField);
        }

        internal static ILGenerator EmitOverride(this TypeBuilder typeBuilder, Type baseType, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField, FieldList fields, out LocalBuilder returnField)
        {
            var attrs = MethodAttributes.Virtual;

            if (baseMethod.Attributes.Has(MethodAttributes.Public))
            { attrs |= MethodAttributes.Public; }

            if (baseMethod.Attributes.Has(MethodAttributes.FamANDAssem))
            { attrs |= MethodAttributes.FamANDAssem; }
            
            var builder = typeBuilder.DefineMethod(
                baseMethod.Name,
                attrs,            
                baseMethod.ReturnType,
                baseMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            var gen = builder.GetILGenerator();

            returnField = baseMethod.ReturnType != typeof(void) ?
                gen.DeclareLocal(baseMethod.ReturnType) : 
                null;

            //gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, interceptorField);

            var @params =
                gen.EmitParameters(baseType, fields, newMethod, baseMethod);

            gen.EmitCall(newMethod, @params);

            return gen;
        }

        internal static FieldBuilder DefineFieldFromProperty(this TypeBuilder builder, PropertyInfo prop)
        {
            var fieldName = prop.Name;

            fieldName = string.Concat(
                '_', Char.ToLower(fieldName[0]), fieldName.Substring(1));

            return builder.DefineField(fieldName, prop.PropertyType, FieldAttributes.Private);
        }

        internal static PropertyBuilder DefineStateBagProperty(this TypeBuilder builder)
        {
            return builder.DefineProperty<object>("StateBag", 
                (bldr, field) =>
                {
                    var dynamicAttrCtor = typeof(DynamicAttribute)
                        .GetConstructor(Type.EmptyTypes);
                    
                    field.SetCustomAttribute(
                        new CustomAttributeBuilder(dynamicAttrCtor, new object[] { }));
                    
                    var statePropBldr = bldr.DefineProperty(
                        "StateBag", PropertyAttributes.HasDefault, typeof(object), null);

                    statePropBldr.SetCustomAttribute(
                        new CustomAttributeBuilder(dynamicAttrCtor, new object[] { }));
                    
                    return statePropBldr;
                });
        }

        internal static PropertyBuilder DefineContainerProperty(this TypeBuilder builder)
        {
            return builder.DefineProperty<SurrogatesContainer>("Container",
                (bldr, field) =>
                    bldr.DefineProperty(
                        "Container", PropertyAttributes.HasDefault, typeof(SurrogatesContainer), null));
        }
    }
}