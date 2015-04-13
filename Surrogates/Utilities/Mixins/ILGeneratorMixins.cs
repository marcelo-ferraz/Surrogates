using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Utilities;
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
        private static bool EmitArgumentsBasedOnOriginal(this ILGenerator gen, MethodInfo originalMethod, ParameterInfo param, Type pType)
        {
            // get the method name if the parameter is named methodname
            if (pType == typeof(string) && param.Name == "s_name")
            {
                gen.Emit(OpCodes.Ldstr, originalMethod.Name);
                return true;
            }

            var baseParams =
                originalMethod.GetParameters();

            if (Try2Add.ArgsParam(gen, param, pType, baseParams))
            { return true; }

            if (Try2Add.OriginalMethodAsParameter(gen, originalMethod, param))
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

            gen.EmitDefaultParameterValue(type, local);

            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br_S, local);
            gen.Emit(OpCodes.Ldloc, local);
        }

        internal static Type[] EmitParameters(this ILGenerator gen, Strategy strategy, MethodInfo method, Func<ParameterInfo, bool> interfere = null)
        {
            var newParams = new List<Type>();

            foreach (var param in method.GetParameters())
            {
                var pType =
                    param.ParameterType;

                newParams.Add(pType);

                // get the instance if the parameter of the interceptor is named instance
                if (pType.IsAssignableFrom(strategy.BaseType) && param.Name == "s_instance")
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    continue;
                }
                
                if (interfere != null && interfere(param))
                { continue; }

                if (Try2Add.AnythingElseAsParameter(gen, strategy.BaseType, strategy.Fields, strategy.NewProperties, param, pType))
                { continue; }
            }
            return newParams.ToArray();
        }

        internal static Type[] EmitParametersForSelf(this ILGenerator gen, Strategy strategy, MethodInfo baseMethod)
        {
            return gen.EmitParameters(strategy, baseMethod, baseMethod);
        }

        internal static Type[] EmitParameters(this ILGenerator gen, Strategy strategy, MethodInfo method, MethodInfo baseMethod)
        {
            return gen.EmitParameters(
                strategy,
                method,
                p => gen.EmitArgumentsBasedOnOriginal(baseMethod, p, p.ParameterType)); 
        }

        /// <summary>
        /// Emits a constructor for the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gen"></param>
        /// <param name="fields"></param>
        internal static void EmitConstructor(this ILGenerator gen, Strategies strategies, params Type[] types)
        {
            var baseParams =
                strategies.BaseType.GetConstructor(types).GetParameters();

            int i = 0;

            gen.Emit(OpCodes.Ldarg_0);

            for (; i < baseParams.Length; i++)
            {
                gen.Emit(OpCodes.Ldarg, i + 1);
            }

            gen.Emit(OpCodes.Call, strategies.BaseType.GetConstructor(types.Length < 1 ? Type.EmptyTypes : types));

            for (int j = 0; j < strategies.Fields.Count; j++)
            {
                var type =
                    strategies.Fields[j].FieldType;

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { }));
                gen.Emit(OpCodes.Stfld, strategies.Fields[j]);
            }

            for (int j = 0; j < strategies.NewProperties.Count; j++)
            {
                var type =
                    strategies.NewProperties[j].GetBuilder().PropertyType;

                //gen.Emit(OpCodes.Ldarg_0);
                //gen.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { }));
                //?? gen.Emit(OpCodes.Stfld, strategies.NewProperties[j]);
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