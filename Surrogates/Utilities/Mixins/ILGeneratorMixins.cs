using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Mappers.Collections;

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

            if (TryAddTheMethodAsParameter(gen, originalMethod, param, baseParams))
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

        /// <summary>
        /// Tries to add the current method as a parameter
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="baseMethod"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        private static bool TryAddTheMethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param, ParameterInfo[] baseParams)
        {
            if (param.Name != "s_method" && 
                param.Name.ToLower() == string.Concat("s_", baseMethod.Name.ToLower())) 
            { return false; }

            if (!baseMethod.IsFamily && !baseMethod.IsPrivate && !baseMethod.IsPublic)
            { throw new NotSupportedException("You cannot use an internal method to be passed as a parameter."); }

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
            if (pType != typeof(object[]) || param.Name != "s_arguments")
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
        /// Emits the default value for varios types
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

        internal static Type[] EmitParameters4<TBase>(this ILGenerator gen, MethodInfo newMethod, Func<ParameterInfo, bool> interfere = null)
        {
            var newParams = new List<Type>();

            foreach (var param in newMethod.GetParameters())
            {
                var pType =
                    param.ParameterType;

                newParams.Add(pType);

                // get the instance if the parameter of the interceptor is named instance
                if (pType.IsAssignableFrom(typeof(TBase)) && param.Name == "s_instance")
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    continue;
                }
                
                if (interfere != null && interfere(param))
                { continue; }
                
                if (!pType.IsValueType)
                { gen.Emit(OpCodes.Ldnull); }
                else
                {
                    gen.EmitDefaultParameterValue(pType);
                }
            }
            return newParams.ToArray();
        }

        internal static Type[] EmitParameters4<TBase>(this ILGenerator gen, MethodInfo newMethod, MethodInfo baseMethod)
        {
            return gen.EmitParameters4<TBase>(
                newMethod,
                p =>  EmitArgumentsBasedOnOriginal(gen, baseMethod, p, p.ParameterType)); 
        }

        /// <summary>
        /// Emits a constructor for the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gen"></param>
        /// <param name="fields"></param>
        internal static void EmitConstructor4<T>(this ILGenerator gen, FieldList fields)
        {
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, typeof(T).GetConstructor(Type.EmptyTypes));

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