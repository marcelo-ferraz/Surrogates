using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Surrogates.Utilities.Mixins;

namespace Surrogates.Utilities
{
    internal static class Try2Add
    {
        internal static bool AnythingElseAsParameter(ILGenerator gen, Type baseType, FieldList fields, ParameterInfo param, Type pType)
        {
            var isSpecialParam =
                param.Name[0] == 's' && param.Name[1] == '_';

            // tries to add any method as parameter 
            if (isSpecialParam && Try2Add.AnyMethodAsParameter(gen, baseType, param))
            { return true; }

            // tries to add any field as parameter 
            if (isSpecialParam && Try2Add.AnyFieldAsParameter(gen, baseType, fields, param, pType))
            { return true; }

            // tries to add any property as parameter 
            if (isSpecialParam && Try2Add.AnyPropertyAsParameter(gen, baseType, fields, param, pType))
            { return true; }

            if (!pType.IsValueType)
            { gen.Emit(OpCodes.Ldnull); }
            else
            { gen.EmitDefaultParameterValue(pType); }

            return false;
        }

        internal static bool MethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param)
        {
            if (param.Name.ToLower() == string.Concat("s_", baseMethod.Name.ToLower()))
            {
                return false;
            }

            if (!baseMethod.IsFamily && !baseMethod.IsPrivate && !baseMethod.IsPublic)
            { throw new NotSupportedException("You cannot use an internal property to be passed as a parameter."); }

            var isFunc =
                baseMethod.ReturnType != typeof(void);

            Type delType =
                Infer.DelegateTypeFrom(baseMethod);

            if (param.ParameterType == typeof(Delegate) || param.ParameterType == delType)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldftn, baseMethod);
                gen.Emit(OpCodes.Newobj, delType.GetConstructors()[0]);

                return true;
            }

            return false;
        }

        internal static bool AnyMethodAsParameter(ILGenerator gen, Type baseType, ParameterInfo param)
        {
            var method = baseType.GetMethod4Surrogacy(
                param.Name.Substring(2), throwExWhenNotFound: false);

            return method != null ?
                Try2Add.MethodAsParameter(gen, method, param) :
                false;
        }

        /// <summary>
        /// Tries to add the current method as a parameter
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="baseMethod"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        internal static bool OriginalMethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param)
        {
            if (param.Name != "s_method")
            { return false; }

            return Try2Add.MethodAsParameter(gen, baseMethod, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        internal static bool ArgsParam(ILGenerator gen, ParameterInfo param, Type pType, ParameterInfo[] baseParams)
        {
            if (pType != typeof(object[]) && param.Name != "s_arguments" && param.Name != "s_args")
            {
                return false;
            }

            var arguments =
                gen.DeclareLocal(typeof(object[]));

            gen.Emit(OpCodes.Ldc_I4, baseParams.Length);
            gen.Emit(OpCodes.Newarr, typeof(object));

            if (baseParams.Length < 1) { return true; }

            gen.Emit(OpCodes.Stloc_0);

            for (int i = 0; i < baseParams.Length; i++)
            {
                gen.Emit(OpCodes.Ldloc_0);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldarg, i + 1);

                if (baseParams[i].ParameterType.IsValueType)
                {
                    gen.Emit(
                        OpCodes.Box,
                        baseParams[i].ParameterType);
                }

                gen.Emit(OpCodes.Stelem_Ref);
            }

            gen.Emit(OpCodes.Ldloc_0);

            return true;
        }

        internal static bool AnyPropertyAsParameter(ILGenerator gen, Type baseType, FieldList fields, ParameterInfo param, Type pType)
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