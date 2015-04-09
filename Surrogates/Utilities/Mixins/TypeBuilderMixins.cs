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
        internal static void CreateConstructor4<T>(this TypeBuilder typeBuilder, FieldList fields)
        {
            typeBuilder.CreateConstructor(typeof(T), fields);
        }

        internal static void CreateConstructor(this TypeBuilder typeBuilder, Type baseType, FieldList fields)
        {
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { });

            ctorBuilder
                .GetILGenerator()
                .EmitConstructor(baseType, fields);
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
            
            attrs |= ((baseMethod.Attributes | MethodAttributes.Public) != MethodAttributes.Public ? MethodAttributes.Public : MethodAttributes.FamANDAssem);
            
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
        
        internal static PropertyBuilder DefinePropertyStateBag(this TypeBuilder builder)
        {
            var dynamicAttrCtor = typeof(DynamicAttribute)
                .GetConstructor(Type.EmptyTypes);
            
            var dynamicAttrBldr = new CustomAttributeBuilder(dynamicAttrCtor, new object[] {});
            
            FieldBuilder StateBldr = builder.DefineField(
                "_stateBag", typeof(string), FieldAttributes.Private);

            StateBldr.SetCustomAttribute(dynamicAttrBldr);

            // The last argument of DefineProperty is null, because the 
            // property has no parameters. (If you don't specify null, you must 
            // specify an array of Type objects. For a parameterless property, 
            // use an array with no elements: new Type[] {})
            PropertyBuilder statePropBldr = 
                builder.DefineProperty(
                "StateBag",
                PropertyAttributes.HasDefault,
                typeof(object),
                null);

            statePropBldr.SetCustomAttribute(dynamicAttrBldr);

            // The property set and property get methods require a special 
            // set of attributes.
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder stateGetPropMthdBldr = builder.DefineMethod(
                "get_StateBag", getSetAttr, typeof(object), Type.EmptyTypes);

            ILGenerator getIL = 
                stateGetPropMthdBldr.GetILGenerator();

            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, StateBldr);
            getIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for CustomerName.
            MethodBuilder stateSetPropMthdBldr = builder.DefineMethod(
                "set_StateBag", getSetAttr, null, new Type[] { typeof(object) });

            ILGenerator setIL = 
                stateSetPropMthdBldr.GetILGenerator();

            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, StateBldr);
            setIL.Emit(OpCodes.Ret);

            // Last, we must map the two methods created above to our PropertyBuilder to  
            // their corresponding behaviors, "get" and "set" respectively. 
            statePropBldr.SetGetMethod(stateGetPropMthdBldr);
            statePropBldr.SetSetMethod(stateSetPropMthdBldr);
            
            return statePropBldr;
        }
    }
}