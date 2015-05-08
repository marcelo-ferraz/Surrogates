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

            if (isSpecialParam && 
                param.ParameterType == typeof(object[]) && 
                (param.Name == "s_arguments" || param.Name == "s_args"))
            {
                gen.Emit(OpCodes.Ldloc, interceptor.ArgsLocal);
            }

            // tries to add any method as parameter 
            //if (isSpecialParam && 
            //    strategy.Accesses.HasFlag(Access.AnyMethod) &&
            //    Try2Add.AnyMethodAsParameter(gen, strategy.BaseType, param, strategy.BaseMethods.Field))
            //{ return true; }

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
                gen.Emit(OpCodes.Ldstr, strategy.ThisDynamic_Type.FullName);
                gen.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetType", new [] { typeof(string) }), new [] { typeof(string) });
                gen.Emit(OpCodes.Ldloc, interceptor.ThisDynamic_Local);
                gen.EmitCall(OpCodes.Call, typeof(Activator).GetMethod("CreateInstance", new[] { typeof(Type), typeof(object[]) }), new[] { typeof(Type), typeof(object[]) });
        
                return true;
            }

            gen.EmitDefaultParameterValue(param.ParameterType); 

            return false;
        }

        internal static void InitializeThisDynamic_(ILGenerator gen, Strategy strategy, Strategy.Interceptor interceptor, MethodInfo originalMethod, ParameterInfo param)
        {
            bool canSeeContainer =
                   strategy.Accesses.HasFlag(Access.Container);
            bool canSeeStateBag =
                strategy.Accesses.HasFlag(Access.StateBag);
            bool canSeeInstance =
                strategy.Accesses.HasFlag(Access.Instance);

            int offset = (canSeeInstance ? 1 : 0) + (canSeeContainer ? 1 : 0) + (canSeeStateBag ? 1 : 0);

            gen.Emit(OpCodes.Ldc_I4, 4 + offset);
            gen.Emit(OpCodes.Newarr, typeof(object));
            gen.Emit(OpCodes.Stloc, interceptor.ThisDynamic_Local);

            Action<int, Action> emitAdd = 
                (i, act)  =>
                {
                    gen.Emit(OpCodes.Ldloc, interceptor.ThisDynamic_Local);
                    gen.Emit(OpCodes.Ldc_I4, i);
                    act();
                    gen.Emit(OpCodes.Stelem_Ref);
                };

            //BaseContainer4Surrogacy container, 
            if (canSeeContainer)
            {
                emitAdd(0, () =>
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.EmitCall(strategy.ContainerProperty.GetBuilder().GetGetMethod());
                });
            }

            ////object bag, 
            if (canSeeStateBag)
            {
                emitAdd(canSeeContainer ? 1 : 0, () =>
                {
                    gen.Emit(OpCodes.Ldarg_0);
                    gen.EmitCall(strategy.StateBagProperty.GetBuilder().GetGetMethod());
                });
            }

            ////object instance, 
            if (canSeeInstance)
            {
                emitAdd(
                    (canSeeContainer ? 1 : 0) + (canSeeStateBag ? 1 : 0),
                    () => gen.Emit(OpCodes.Ldarg_0));
            }

            ////string methodName,
            emitAdd(
                offset++,
                () => gen.Emit(OpCodes.Ldstr, originalMethod.Name));

            ////string className, 
            emitAdd(
                offset++,
                () => gen.Emit(OpCodes.Ldstr, strategy.BaseType.FullName));

            //Delegate baseMethod, 
            emitAdd(
                offset++,
                () => gen.Emit(OpCodes.Ldloc, interceptor.S_MethodParam));

            //object[] args
            emitAdd(
                offset++,
                () => gen.Emit(OpCodes.Ldloc, interceptor.ArgsLocal));
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

        internal static bool InitializeMethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param, Strategy.Interceptor interceptor, FieldInfo baseMethodsField)
        {
            if (!baseMethod.IsFamily && !baseMethod.IsPrivate && !baseMethod.IsPublic)
            { throw new NotSupportedException("You cannot use an internal property to be passed as a parameter."); }

            var isFunc =
                baseMethod.ReturnType != typeof(void);

            Type delType =
                Infer.DelegateTypeFrom(baseMethod);

            //if (param.ParameterType == typeof(Delegate) || param.ParameterType == delType)
            {
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

                gen.Emit(OpCodes.Stloc, interceptor.S_MethodParam);
                return true;
            }

            return false;
        }

        internal static bool AnyMethodAsParameter(ILGenerator gen, Type baseType, ParameterInfo param, Strategy.Interceptor interceptor, FieldInfo baseMethodsField)
        {
            var method = baseType.GetMethod4Surrogacy(
                param.Name.Substring(2), throwExWhenNotFound: false);

            if (method == null || param.Name.ToLower() != string.Concat("s_", method.Name.ToLower()))
            {
                return false;
            }

            return method != null ?
                Try2Add.InitializeMethodAsParameter(gen, method, param, interceptor, baseMethodsField) :
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
        internal static bool InitializeOriginalMethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param, Strategy.Interceptor interceptor, FieldInfo baseMethodsField)
        {
            return Try2Add.InitializeMethodAsParameter(gen, baseMethod, param, interceptor, baseMethodsField);
        }

        /// <summary>
        /// Adds all parameters as an array of objects
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        internal static bool InitializeArgsParam(ILGenerator gen, ParameterInfo param, Strategy.Interceptor interceptor, ParameterInfo[] baseParams)
        {
            //original that works
            var arguments =
                gen.DeclareLocal(typeof(object[]));

            gen.Emit(OpCodes.Ldc_I4, baseParams.Length);
            gen.Emit(OpCodes.Newarr, typeof(object));
            gen.Emit(OpCodes.Stloc, interceptor.ArgsLocal);

            if (baseParams.Length < 1) { return true; }

            for (int i = 0; i < baseParams.Length; i++)
            {
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