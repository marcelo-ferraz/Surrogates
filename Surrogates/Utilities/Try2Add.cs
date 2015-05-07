using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Surrogates.Utilities.Mixins;
using Surrogates.Tactics;

namespace Surrogates.Utilities
{
    internal static class Try2Add
    {
        internal static bool AnythingAsParameter(ILGenerator gen, Strategy strategy, Strategy.Interceptor interceptor, MethodInfo originalMethod, ParameterInfo param)
        {
            var isSpecialParam =
                param.Name[0] == 's' && param.Name[1] == '_';
            
            if (param.ParameterType == typeof(string) && param.Name == "s_name")
            {
                gen.Emit(OpCodes.Ldstr, originalMethod.Name);
                return true;
            }

            // get the instance if the parameter of the interceptor is named instance
            if (isSpecialParam && 
                strategy.Accesses.HasFlag(Access.Instance) &&
                InstanceAsParameter(gen, strategy, param))
            { return true; }

            // tries to add any method as parameter 
            if (isSpecialParam && 
                strategy.Accesses.HasFlag(Access.AnyMethod) &&
                Try2Add.AnyMethodAsParameter(gen, strategy.BaseType, param, strategy.BaseMethods.Field))
            { return true; }

            // tries to add any field as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyField) &&
                Try2Add.AnyFieldAsParameter(gen, strategy.BaseType, strategy.Fields, param, param.ParameterType))
            { return true; }

            // tries to add any property as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyBaseProperty) &&
                Try2Add.AnyBasePropertyAsParameter(gen, strategy.BaseType, strategy.Fields, param, param.ParameterType))
            { return true; }
            
            // tries to add any property as parameter 
            if (isSpecialParam &&
                strategy.Accesses.HasFlag(Access.AnyNewProperty) &&
                Try2Add.AnyNewPropertyAsParameter(gen, strategy.BaseType, strategy.NewProperties, param, param.ParameterType))
            { return true; }

            if (param.Name[0] == '_' && param.ParameterType == typeof(object))
            {
                bool canSeeContainer = strategy.Accesses.HasFlag(Access.Container);
                bool canSeeStateBag = strategy.Accesses.HasFlag(Access.StateBag);
                bool canSeeInstance = strategy.Accesses.HasFlag(Access.Instance);
                int offset = (canSeeInstance?1:0) + (canSeeContainer?1:0) + (canSeeStateBag?1:0);

                var ldloc = interceptor.ArgsLocal == null?
                    OpCodes.Ldloc_0 : OpCodes.Ldloc_1;

                var stloc = interceptor.ArgsLocal == null?
                    OpCodes.Stloc_0 : OpCodes.Stloc_1;


                gen.Emit(OpCodes.Ldc_I4, (sbyte)(4 + offset));
                gen.Emit(OpCodes.Newarr, typeof(object));
                gen.Emit(stloc);

                //BaseContainer4Surrogacy container, 
                if (canSeeContainer)
                {
                    gen.Emit(ldloc);
                    gen.Emit(OpCodes.Ldc_I4_0);
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.EmitCall(strategy.ContainerProperty.GetBuilder().GetGetMethod());
                    gen.Emit(OpCodes.Stelem_Ref);
                }
                
                //object bag, 
                if (canSeeStateBag)
                {
                    gen.Emit(ldloc);
                    gen.Emit(OpCodes.Ldc_I4, canSeeContainer ? 1 : 0);
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.EmitCall(strategy.StateBagProperty.GetBuilder().GetGetMethod());
                    gen.Emit(OpCodes.Stelem_Ref);
                }
                
                //object instance, 
                if (canSeeInstance)
                {
                    gen.Emit(ldloc);
                    gen.Emit(OpCodes.Ldc_I4, (canSeeContainer ? 1 : 0) + (canSeeStateBag ? 1 : 0));
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.Emit(OpCodes.Stelem_Ref);
                }
                //string methodName,
                gen.Emit(ldloc);
                gen.Emit(OpCodes.Ldc_I4, offset++);
                gen.Emit(OpCodes.Ldstr, originalMethod.Name);
                gen.Emit(OpCodes.Stelem_Ref);

                //string className, 
                gen.Emit(ldloc);
                gen.Emit(OpCodes.Ldc_I4, offset++);
                gen.Emit(OpCodes.Ldstr, strategy.BaseType.FullName);
                gen.Emit(OpCodes.Stelem_Ref);

                //Delegate baseMethod, 
                gen.Emit(ldloc);
                gen.Emit(OpCodes.Ldc_I4, offset++);
                MethodAsParameter(gen, interceptor.Method, param, strategy.BaseMethods.Field);
                gen.Emit(OpCodes.Stelem_Ref);
                //object[] args
                gen.Emit(ldloc);
                gen.Emit(OpCodes.Ldc_I4, offset++);
                ArgsParam(gen, param, 0, param.ParameterType, interceptor.Method.GetParameters());
                gen.Emit(OpCodes.Stelem_Ref);

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, strategy.ThisDynamic_Type.FullName);
                gen.EmitCall(typeof(Type).GetMethod("GetType", new [] { typeof(string) }), new [] { typeof(string) });
                gen.Emit(ldloc);
                gen.EmitCall(OpCodes.Call, typeof(Activator).GetMethod("CreateInstance", new[] { typeof(Type), typeof(object[]) }), new[] { typeof(Type), typeof(object[]) });
        
                //gen.Emit(OpCodes.Call, strategy.ThisBig_Type.GetConstructors()[0]);

            }

            gen.EmitDefaultParameterValue(param.ParameterType); 

            return false;
        }


        private static bool InstanceAsParameter(ILGenerator gen, Strategy strategy, ParameterInfo param)
        {
            if (param.ParameterType.IsAssignableFrom(strategy.BaseType) && param.Name == "s_instance")
            {
                gen.Emit(OpCodes.Ldarg_0);
                return true;
            }
            return false;
        }

        internal static bool MethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param, FieldInfo baseMethodsField)
        {
            if (!baseMethod.IsFamily && !baseMethod.IsPrivate && !baseMethod.IsPublic)
            { throw new NotSupportedException("You cannot use an internal property to be passed as a parameter."); }

            var isFunc =
                baseMethod.ReturnType != typeof(void);

            Type delType =
                Infer.DelegateTypeFrom(baseMethod);

            if (param.ParameterType == typeof(Delegate) || param.ParameterType == delType)
            {
                //gen.Emit(OpCodes.Ldarg_0);
                //gen.Emit(OpCodes.Dup);
                                
                //gen.Emit(OpCodes.Ldvirtftn, baseMethod);
                ////gen.Emit(OpCodes.Ldftn, baseMethod);                
                //gen.Emit(OpCodes.Newobj, delType.GetConstructors()[0]);

                //IL_0018: ldsfld class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [mscorlib]System.Func`2<object, class [mscorlib]System.Delegate>> Surrogates.Applications.Tests.SimpleProxy3::_baseMethods
                gen.Emit(OpCodes.Ldsfld, baseMethodsField);
                //IL_001d: ldstr "Add2List"
                gen.Emit(OpCodes.Ldstr, baseMethod.Name);
                //IL_0022: callvirt instance class [mscorlib]System.Func`2<object, class [mscorlib]System.Delegate> class [mscorlib]System.Collections.Generic.Dictionary`2<string, class [mscorlib]System.Func`2<object, class [mscorlib]System.Delegate>>::get_Item(!0)
                gen.EmitCall(typeof(Dictionary<string, Func<object, Delegate>>).GetMethod4Surrogacy("get_Item"));
                //IL_0027: ldarg.0
                gen.Emit(OpCodes.Ldarg_0);
                //IL_0028: callvirt instance class [mscorlib]System.Delegate class [mscorlib]System.Func`2<object, class [mscorlib]System.Delegate>::Invoke(!0)
                gen.EmitCall(typeof(Func<object, Delegate>).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance), new [] { typeof(object) });

                return true;
            }

            return false;
        }

        internal static bool AnyMethodAsParameter(ILGenerator gen, Type baseType, ParameterInfo param, FieldInfo baseMethodsField)
        {
            var method = baseType.GetMethod4Surrogacy(
                param.Name.Substring(2), throwExWhenNotFound: false);

            if (method == null || param.Name.ToLower() != string.Concat("s_", method.Name.ToLower()))
            {
                return false;
            }

            return method != null ?
                Try2Add.MethodAsParameter(gen, method, param, baseMethodsField) :
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
        internal static bool OriginalMethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param, FieldInfo baseMethodsField)
        {
            if (param.Name != "s_method")
            { return false; }

            return Try2Add.MethodAsParameter(gen, baseMethod, param, baseMethodsField);
        }

        /// <summary>
        /// Adds all parameters as an array of objects
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        internal static bool ArgsParam(ILGenerator gen, ParameterInfo param, int paramIndex, Type pType, ParameterInfo[] baseParams)
        {
            if (pType != typeof(object[]) && param.Name != "s_arguments" && param.Name != "s_args")
            {
                return false;
            }

            //var arguments =
            //    gen.DeclareLocal(typeof(object[]));

            //gen.Emit(OpCodes.Ldc_I4, baseParams.Length);
            //gen.Emit(OpCodes.Newarr, typeof(object));

            //if (baseParams.Length < 1) { return true; }

            //gen.EmitStloc(paramIndex);

            //for (int i = 0; i < baseParams.Length; i++)
            //{
            //    gen.EmitLdloc(i + 1);
            //    gen.Emit(OpCodes.Ldc_I4, i);

            //    gen.EmitArg(i + 1);


            //    if (baseParams[i].ParameterType.IsValueType)
            //    {
            //        gen.Emit(
            //            OpCodes.Box,
            //            baseParams[i].ParameterType);
            //    }

            //    gen.Emit(OpCodes.Stelem_Ref);
            //}

            //gen.Emit(OpCodes.Ldloc_1);

            //return true;
            //original that works
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

        internal static bool AnyBasePropertyAsParameter(ILGenerator gen, Type baseType, FieldList fields, ParameterInfo param, Type pType)
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