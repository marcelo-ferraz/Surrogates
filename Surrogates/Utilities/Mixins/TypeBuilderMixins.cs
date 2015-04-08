using Surrogates.Model.Collections;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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

            var ctrGen = ctorBuilder.GetILGenerator();

            ctrGen.EmitConstructor(baseType, fields);
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
                attrs
                //MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask,	
,
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
    }
}