using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates
{
    using Surrogates.Mappers.Collections;
    using EmitBasedOnOrginal = Action<ILGenerator, MethodInfo, ParameterInfo, Type>;

    internal static class ILGeneratorMixins
    {
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
                if (pType.IsAssignableFrom(typeof(TBase)) && param.Name == "instance")
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

        internal static Type[] EmitParameters4<TBase>(this ILGenerator gen, MethodInfo newMethod, string paramName, string literalValue)
        {
            return gen.EmitParameters4<TBase>(
                newMethod,
                p => {
                    if (p.ParameterType == typeof(string) && p.Name == paramName)
                    {
                        gen.Emit(OpCodes.Ldstr, literalValue);
                        return true;
                    }
                    return false;
                });
        }

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
            if (pType == typeof(string) && param.Name == "methodName")
            {
                gen.Emit(OpCodes.Ldstr, originalMethod.Name);
                return true;
            }

            var baseParams =
                originalMethod.GetParameters();

            bool paramFound = false;

            for (int i = 0; i < baseParams.Length; i++)
            {
                if (baseParams[i].Name != param.Name)
                { continue; }

                paramFound =
                    pType.IsAssignableFrom(baseParams[i].ParameterType);

                if (paramFound)
                {
                    gen.Emit(OpCodes.Ldarg, i + 1);
                    break;
                }
            }

            return paramFound;
        }

        internal static void EmitConstructor4<T>(this ILGenerator gen, FieldList fields)
        {
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, typeof(T).GetConstructor(Type.EmptyTypes));
            //gen.Emit(OpCodes.Nop);

            for (int i = 0; i < fields.Count; i++)
            {
                var type =
                    fields[i].FieldType;

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { }));
                gen.Emit(OpCodes.Stfld, fields[i]);
            }

            //gen.Emit(OpCodes.Nop);

            gen.Emit(OpCodes.Ret);
        }
    }
}