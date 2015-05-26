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
        /// 
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="original"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        public static bool EmitArgumentsBasedOnOriginal(this ILGenerator gen, MethodInfo originalMethod, ParameterInfo param, int paramIndex, FieldInfo baseMethodsField)
        {
            var baseParams =
                originalMethod.GetParameters();

            for (int i = 0; i < baseParams.Length; i++)
            {
                if (baseParams[i].Name == param.Name)
                {
                    var compatible =
                        param.ParameterType.IsAssignableFrom(baseParams[i].ParameterType);

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
            if (type == typeof(string))
            {
                gen.Emit(OpCodes.Ldstr, string.Empty);
                return;
            }
            if (type.IsInterface || !type.IsValueType)
            {
                gen.Emit(OpCodes.Ldnull);
                return;
            }

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
                
        internal static Type[] EmitParameters(this ILGenerator gen, Strategy strategy, Strategy.Interceptor interceptor, MethodInfo baseMethod, Func<ParameterInfo, int, bool> interfere = null)
        {
            var newParams = new List<Type>();

            var @params = interceptor.Method.GetParameters();

            for (int i = 0; i < @params.Length; i++)
            {
                var pType =
                    @params[i].ParameterType;

                newParams.Add(pType);
                              
                if (interfere != null && interfere(@params[i], i))
                { continue; }

                Try2Add.AnythingAsParameter(gen, strategy, interceptor, baseMethod, @params[i]);
            }
            return newParams.ToArray();
        }

        internal static Type[] EmitParametersForSelf(this ILGenerator gen, Strategy strategy, MethodInfo baseMethod)
        {
            var @params = baseMethod.GetParameters();
            
            var newParams = new Type[@params.Length];

            for (int i = 0; i < @params.Length; i++)
            {
                newParams[i] = @params[i].ParameterType;

                gen.EmitArgumentsBasedOnOriginal(baseMethod, @params[i], i, strategy.BaseMethods.Field);
            }

            return newParams;
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
                var b = strategies.NewProperties[j].GetBuilder();

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Newobj, b.PropertyType.GetConstructor(Type.EmptyTypes));
                gen.EmitCall(b.GetSetMethod());
            }

            int i = 0;

            gen.Emit(OpCodes.Ldarg_0);

            for (; i < baseParams.Length; i++)
            {
                gen.Emit(OpCodes.Ldarg, i + 1);
            }

            gen.Emit(OpCodes.Call, strategies.BaseType.GetConstructor(types.Length < 1 ? Type.EmptyTypes : types));
            
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
            try
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
            catch (InvalidOperationException ex)
            {
                if(ex.Message == "Calling convention must be VarArgs.")
                {
                    gen.Emit(OpCodes.Call, method);
                }
            }
        }

        internal static void EmitArg(this ILGenerator gen, int index) 
        {
            if (index <= 3)
            {
                gen.Emit(
                    index == 0 ? OpCodes.Ldarg_0 :
                    index == 1 ? OpCodes.Ldarg_1 :
                    index == 2 ? OpCodes.Ldarg_2 :
                    OpCodes.Ldarg_3);
            }
            else 
            { gen.Emit(OpCodes.Ldarg, index); }
        }

        internal static void EmitStloc(this ILGenerator gen, int index)
        {
            if (index <= 3)
            {
                gen.Emit(
                    index == 0 ? OpCodes.Stloc_0 :
                    index == 1 ? OpCodes.Stloc_1 :
                    index == 2 ? OpCodes.Stloc_2 :
                    OpCodes.Stloc_3);
            }
            //else { //TODO: discover what to do }
        }

        internal static void EmitLdloc(this ILGenerator gen, int index)
        {
            if (index <= 3)
            {
                gen.Emit(
                    index == 0 ? OpCodes.Ldloc_0 :
                    index == 1 ? OpCodes.Ldloc_1 :
                    index == 2 ? OpCodes.Ldloc_2 :
                    OpCodes.Ldloc_3);
            }
            //else { //TODO: discover what to do }
        }
    }
}