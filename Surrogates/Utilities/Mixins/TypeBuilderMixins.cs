using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Mappers.Collections;

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

        internal static ILGenerator EmitOverride<TBase>(this TypeBuilder typeBuilder, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField)
        {
            return EmitOverride(typeBuilder, typeof(TBase), newMethod, baseMethod, interceptorField);
        }

        internal static ILGenerator EmitOverride(this TypeBuilder typeBuilder, Type baseType, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField)
        {
            LocalBuilder @return = null;
            return EmitOverride(typeBuilder, baseType, newMethod, baseMethod, interceptorField, out @return);
        }

        internal static ILGenerator EmitOverride<TBase>(this TypeBuilder typeBuilder, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField, out LocalBuilder returnField)
        {
            return EmitOverride(typeBuilder, typeof(TBase), newMethod, baseMethod, interceptorField, out returnField);
        }

        internal static ILGenerator EmitOverride(this TypeBuilder typeBuilder, Type baseType, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField, out LocalBuilder returnField)
        {
            var builder = typeBuilder.DefineMethod(
                baseMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
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
                gen.EmitParameters(baseType, newMethod, baseMethod);

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