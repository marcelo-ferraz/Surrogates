using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates
{
    internal static class ILGeneratorMixins
    {
        /// <summary>
        /// Emits the default value for varios types
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="type"></param>
        internal static void EmitDefaultValue(this ILGenerator gen, Type type, LocalBuilder local = null)
        {
            bool isInteger =
                type == typeof(sbyte) || type == typeof(byte) ||
                type == typeof(ushort) || type == typeof(short) ||
                type == typeof(uint) || type == typeof(int) ||
                type == typeof(ulong) || type == typeof(long);

            if (local == null)
            {
                local = gen.DeclareLocal(type);
            }
            
            if (type == typeof(DateTime) || type == typeof(TimeSpan))
            {
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
             
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br_S, local);
            gen.Emit(OpCodes.Ldloc, local);
        }

        internal static Type[] EmitParameters(this ILGenerator gen, MethodInfo newMethod, MethodInfo baseMethod)
        {
            var newParams =
                new List<Type>();

            var baseType =
                baseMethod.DeclaringType;

            foreach (var param in newMethod.GetParameters())
            {
                var pType =
                    param.ParameterType;

                newParams.Add(pType);

                // get the instance if the parameter of the interceptor is named instance
                if (pType.IsAssignableFrom(baseType) && param.Name == "instance")
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    continue;
                }

                // get the method name if the parameter is named methodname
                if (pType == typeof(string) && param.Name == "methodName")
                {
                    gen.Emit(OpCodes.Ldstr, baseMethod.Name);
                    continue;
                }

                // get the originl method's parameters if they have the same name and type or are assinable from the type do not forget to box
                var baseParams =
                    baseMethod.GetParameters();

                bool paramFound = false;

                for (int i = 0; i < baseParams.Length; i++)
                {
                    if (baseParams[i].Name != param.Name)
                    { continue; }

                    paramFound =
                        pType.IsAssignableFrom(baseParams[i].ParameterType);

                    if (paramFound)
                    {
                        //gen.EmitWriteLine("emitting:= " + (i + 1).ToString());
                        
                        // original code (wich surprisingly did not work):
                        gen.Emit(OpCodes.Ldarg, i + 1);
                        //switch (i)
                        //{
                        //    case 0: gen.Emit(OpCodes.Ldarg_1); break;
                        //    case 1: gen.Emit(OpCodes.Ldarg_2); break;
                        //    case 2: gen.Emit(OpCodes.Ldarg_3); break;
                        //}

                        break;
                    }
                }

                if (paramFound) { continue; }

                if (!pType.IsValueType)
                { gen.Emit(OpCodes.Ldnull); }
                else
                {
                    gen.EmitDefaultValue(pType);
                }
            }
            return newParams.ToArray();
        }

        internal static void EmitConstructor4<T>(this ILGenerator gen, IList<FieldInfo> fields)
        {
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, typeof(T).GetConstructor(new Type[] { }));
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