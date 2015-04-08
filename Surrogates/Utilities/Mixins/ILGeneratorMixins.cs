using Surrogates.Model.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities.Mixins
{
    internal static class ILGeneratorMixins
    {
        /// <summary>
        /// Setter the original method's parameters if they have the same name and type or are assinable from the type do not forget to 
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="original"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        private static bool EmitArgumentsBasedOnOriginal(ILGenerator gen, MethodInfo originalMethod, ParameterInfo param, Type pType)
        {
            // get the method name if the parameter is named methodname
            if (pType == typeof(string) && param.Name == "s_name")
            {
                gen.Emit(OpCodes.Ldstr, originalMethod.Name);
                return true;
            }

            var baseParams =
                originalMethod.GetParameters();
            
            if (TryAddArgsParam(gen, param, pType, baseParams))
            { return true; }

            if (TryAddTheMethodAsParameter(gen, originalMethod, param))
            { return true; }
            
            for (int i = 0; i < baseParams.Length; i++)
            {
                if (baseParams[i].Name == param.Name)
                {
                    var compatible =
                        pType.IsAssignableFrom(baseParams[i].ParameterType);

                    if (compatible)
                    {
                        gen.Emit(OpCodes.Ldarg, i + 1);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryAddMethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param)
        {
            if (param.Name.ToLower() == string.Concat("s_", baseMethod.Name.ToLower()))
            {  
                return false; 
            }

            if (!baseMethod.IsFamily && !baseMethod.IsPrivate && !baseMethod.IsPublic)
            { throw new NotSupportedException("You cannot use an internal property to be passed as a parameter."); }

            var isFunc =
                baseMethod.ReturnType != typeof(void);

            Type delType = Infer.DelegateTypeFrom(baseMethod);

            if (param.ParameterType == typeof(Delegate) || param.ParameterType == delType)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldftn, baseMethod);
                gen.Emit(OpCodes.Newobj, delType.GetConstructors()[0]);

                return true;
            }

            return false;
        }

        private static bool TryPassAnyMethodAsParameter(ILGenerator gen, Type baseType, ParameterInfo param)
        {
            var method = baseType.GetMethod4Surrogacy(
                param.Name.Substring(2), throwExWhenNotFound: false);

            return method != null ?
                TryAddMethodAsParameter(gen, method, param) : 
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
        private static bool TryAddTheMethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param)
        {
            if (param.Name != "s_method") 
            { return false; }

            return TryAddMethodAsParameter(gen, baseMethod, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        private static bool TryAddArgsParam(ILGenerator gen, ParameterInfo param, Type pType, ParameterInfo[] baseParams)
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
        
        private static bool TryPassAnyPropertyAsParameter(ILGenerator gen, Type baseType, FieldList fields, ParameterInfo param, Type pType)
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

        private static bool TryPassAnyFieldAsParameter(ILGenerator gen, Type baseType, FieldList fields, ParameterInfo param, Type pType)
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
        

        internal static void EmitDefaultParameterValue(this ILGenerator gen, Type type, LocalBuilder local = null)
        {
            bool isInteger =
                type == typeof(sbyte) || type == typeof(byte) ||
                type == typeof(ushort) || type == typeof(short) ||
                type == typeof(uint) || type == typeof(int) ||
                type == typeof(ulong) || type == typeof(long);
          
            if (type == typeof(DateTime) || type == typeof(TimeSpan))
            {
                if (local == null)
                { local = gen.DeclareLocal(type); }

                gen.Emit(OpCodes.Ldloca_S, local);
                gen.Emit(OpCodes.Initobj, type);
                gen.Emit(OpCodes.Ldloc, local);
                return;
            }

            if (isInteger || type == typeof(decimal) || type == typeof(char))
            {
                gen.Emit(OpCodes.Ldc_I4_0);
            }
            else if (type == typeof(float) || type == typeof(double))
            {
                gen.Emit(OpCodes.Ldc_R4, 0.0);
            }
            else if (type == typeof(string))
            {
                gen.Emit(OpCodes.Ldstr, string.Empty);
            }
            else { throw new NotSupportedException(string.Format("The type {0} is not supporte, yet.", type)); }
        }
       
        /// <summary>
        /// Emits the default value for various types
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="type"></param>
        internal static void EmitDefaultValue(this ILGenerator gen, Type type, LocalBuilder local = null)
        {
            if (local == null)
            { local = gen.DeclareLocal(type); }

            EmitDefaultParameterValue(gen, type, local);

            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br_S, local);
            gen.Emit(OpCodes.Ldloc, local);
        }

        internal static Type[] EmitParameters(this ILGenerator gen, Type baseType, FieldList fields, MethodInfo newMethod, Func<ParameterInfo, bool> interfere = null)
        {
            var newParams = new List<Type>();

            foreach (var param in newMethod.GetParameters())
            {
                var pType =
                    param.ParameterType;

                newParams.Add(pType);

                // get the instance if the parameter of the interceptor is named instance
                if (pType.IsAssignableFrom(baseType) && param.Name == "s_instance")
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    continue;
                }
                
                if (interfere != null && interfere(param))
                { continue; }


                var isSpecialParam =
                    param.Name[0] == 's' && param.Name[1] == '_';

                // tries to find any method as parameter 
                if (isSpecialParam && TryPassAnyMethodAsParameter(gen, baseType, param))
                { continue; }

                // tries to find any field as parameter 
                if (isSpecialParam && TryPassAnyFieldAsParameter(gen, baseType, fields, param, pType))
                { continue; }

                // tries to find any property as parameter 
                if (isSpecialParam && TryPassAnyPropertyAsParameter(gen, baseType, fields, param, pType))
                { continue; }

                if (!pType.IsValueType)
                { gen.Emit(OpCodes.Ldnull); }
                else
                { gen.EmitDefaultParameterValue(pType); }
            }
            return newParams.ToArray();
        }

        internal static Type[] EmitParametersForSelf(this ILGenerator gen, Type baseType, FieldList fields, MethodInfo baseMethod)
        {
            return EmitParameters(gen, baseType, fields, baseMethod, baseMethod);
        }

        internal static Type[] EmitParameters(this ILGenerator gen, Type baseType, FieldList fields, MethodInfo newMethod, MethodInfo baseMethod)
        {
            return gen.EmitParameters(
                baseType,
                fields,
                newMethod,
                p =>  EmitArgumentsBasedOnOriginal(gen, baseMethod, p, p.ParameterType)); 
        }

        /// <summary>
        /// Emits a constructor for the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gen"></param>
        /// <param name="fields"></param>
        internal static void EmitConstructor(this ILGenerator gen, Type baseType, FieldList fields)
        {
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, baseType.GetConstructor(Type.EmptyTypes));

            for (int i = 0; i < fields.Count; i++)
            {
                var type =
                    fields[i].FieldType;

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { }));
                gen.Emit(OpCodes.Stfld, fields[i]);
            }

            gen.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="method"></param>
        /// <param name="params"></param>
        internal static void EmitCall(this ILGenerator gen, MethodInfo method, Type[] @params = null)
        {
            if ((method.CallingConvention | CallingConventions.VarArgs) == CallingConventions.VarArgs)
            {
                gen.Emit(OpCodes.Call, method);
            }
            else
            {
                gen.EmitCall(OpCodes.Callvirt, method, @params ?? Type.EmptyTypes);
            }
        }
    }
}