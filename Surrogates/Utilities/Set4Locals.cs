using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities
{
    public static class Set4Locals
    {
        public static LocalBuilder AllComplexParameters(Strategy.ForMethods strat, MethodInfo baseMethod, ILGenerator gen)
        {
            LocalBuilder returnField = baseMethod.ReturnType != typeof(void) ?
                gen.DeclareLocal(baseMethod.ReturnType) :
                null;

            foreach (var param in strat.Interceptor.Method.GetParameters())
            {                
                if (param.IsSelfArguments())
                {
                    strat.Interceptor.Locals.Add("Args", 
                        Set4Locals.ArgsParam(gen, param, baseMethod.GetParameters()));
                }

                if (param.IsSelfMethod())
                {
                    strat.Interceptor.Locals.Add("S_Method", 
                        Set4Locals.OriginalMethodAsParameter(gen, baseMethod, param, strat.BaseMethods.Field));
                }

                if (param.IsDynamic_())
                {
                    strat.Interceptor.Locals.Add("ThisDynamic_", 
                        Set4Locals.ThisDynamic_(gen, strat, strat.Interceptor, baseMethod, param));
                }

                if (param.Is4SomeMethod())
                {
                    var key = string.Concat(param.Name, "+", param.ParameterType.Name);

                    strat.Interceptor.Locals.Add(key, 
                        Set4Locals.ThisDynamic_(gen, strat, strat.Interceptor, baseMethod, param));
                }
            }
            return returnField;
        }

        /// <summary>
        /// Adds all parameters as an array of objects
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="param"></param>
        /// <param name="pType"></param>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        internal static LocalBuilder ArgsParam(ILGenerator gen, ParameterInfo param, ParameterInfo[] baseParams)
        {
            var local = gen.DeclareLocal(typeof(object[]));

            var arguments =
                gen.DeclareLocal(typeof(object[]));

            gen.Emit(OpCodes.Ldc_I4, baseParams.Length);
            gen.Emit(OpCodes.Newarr, typeof(object));
            gen.Emit(OpCodes.Stloc, local);

            if (baseParams.Length < 1) { return local; }

            gen.Emit(OpCodes.Ldloc, local);

            for (int i = 0; i < baseParams.Length; i++)
            {
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldarg, i + 1);

                // box the value to be set on the array
                if (baseParams[i].ParameterType.IsValueType)
                {
                    gen.Emit(OpCodes.Box,
                        baseParams[i].ParameterType);
                }

                gen.Emit(OpCodes.Stelem_Ref);
            }

            return local;
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
        internal static LocalBuilder OriginalMethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param, FieldInfo baseMethodsField, bool is4Dynamic_ = false)
        {
            return Set4Locals.MethodAsParameter(gen, baseMethod, param, baseMethodsField, is4Dynamic_);
        }

        /// <summary>
        /// Initializes a given method as a parameter 
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="baseMethod"></param>
        /// <param name="param"></param>
        /// <param name="baseMethodsField"></param>
        /// <param name="isDynamic_"></param>
        /// <returns></returns>
        internal static LocalBuilder MethodAsParameter(ILGenerator gen, MethodInfo baseMethod, ParameterInfo param, FieldInfo baseMethodsField, bool isDynamic_ = false)
        {
            if (!baseMethod.IsFamily && !baseMethod.IsPrivate && !baseMethod.IsPublic)
            { throw new NotSupportedException("You cannot use an internal property to be passed as a parameter."); }

            var local = gen.DeclareLocal(isDynamic_ ? typeof(Delegate) : param.ParameterType);

            var isFunc =
                baseMethod.ReturnType != typeof(void);

            Type delType =
                Infer.DelegateTypeFrom(baseMethod);

            gen.Emit(OpCodes.Ldsfld, baseMethodsField);
            gen.Emit(OpCodes.Ldstr, baseMethod.Name);
            gen.EmitCall(typeof(Dictionary<string, Func<object, Delegate>>).GetMethod4Surrogacy("get_Item"));
            gen.Emit(OpCodes.Ldarg_0);
            gen.EmitCall(typeof(Func<object, Delegate>).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance), new[] { typeof(object) });

            gen.Emit(OpCodes.Stloc, local);

            return local;
        }

        /// <summary>
        /// It passes any method to a local variable and sets it  
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="baseType"></param>
        /// <param name="param"></param>
        /// <param name="baseMethodsField"></param>
        /// <returns></returns>
        internal static LocalBuilder AnyMethodAsParameter(ILGenerator gen, Type baseType, ParameterInfo param, FieldInfo baseMethodsField)
        {
            MethodInfo method = null;

            if (param.ParameterType == typeof(Delegate))
            {
                method = baseType.GetMethod4Surrogacy(
                     param.Name.Substring(2), throwExWhenNotFound: false);
            }
            else if (!param.ParameterType.IsGenericType)
            {
                method = baseType.GetMethod4Surrogacy(
                     param.Name.Substring(2), Type.EmptyTypes, false);
            }
            else
            {
                var genDef = param
                    .ParameterType.GetGenericTypeDefinition();

                var genParams = param
                    .ParameterType.GetGenericArguments();

                if (genDef == typeof(Func<>))
                {
                    method = baseType.GetMethod4Surrogacy(
                         param.Name.Substring(2), Type.EmptyTypes, false);
                }
                else if (param.ParameterType.Name.StartsWith("Func"))
                {
                    method = baseType.GetMethod4Surrogacy(
                        param.Name.Substring(2), genParams.Take(genParams.Length - 1).ToArray(), false);
                }
                else
                {
                    method = baseType.GetMethod4Surrogacy(
                        param.Name.Substring(2), genParams, false);
                }
            }

            return method != null ?
                Set4Locals.MethodAsParameter(gen, method, param, baseMethodsField) :
                null;
        }

        /// <summary>
        /// Creates the local for this dynamic underscore and sets its all values
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="strategy"></param>
        /// <param name="interceptor"></param>
        /// <param name="originalMethod"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        internal static LocalBuilder ThisDynamic_(ILGenerator gen, Strategy strategy, Strategy.InterceptorInfo interceptor, MethodInfo originalMethod, ParameterInfo param)
        {

            if (!interceptor.Locals.ContainsKey("S_Method"))
            { 
                interceptor.Locals.Add("S_Method", 
                    Set4Locals.OriginalMethodAsParameter(gen, originalMethod, param, strategy.BaseMethods.Field, is4Dynamic_: true)); 
            }

            if (!interceptor.Locals.ContainsKey("Args"))
            {
                interceptor.Locals.Add("Args", 
                    Set4Locals.ArgsParam(gen, param, originalMethod.GetParameters()));
            }

            var local =
                gen.DeclareLocal(typeof(object[]));

            bool canSeeContainer =
                   strategy.Accesses.HasFlag(Access.Container);
            bool canSeeStateBag =
                strategy.Accesses.HasFlag(Access.StateBag);
            bool canSeeInstance =
                strategy.Accesses.HasFlag(Access.Instance);

            int offset = (canSeeInstance ? 1 : 0) + (canSeeContainer ? 1 : 0) + (canSeeStateBag ? 1 : 0);

            gen.Emit(OpCodes.Ldc_I4, 4 + offset);
            gen.Emit(OpCodes.Newarr, typeof(object));
            gen.Emit(OpCodes.Stloc, local);

            Action<int, Action> emitAdd =
                (i, act) =>
                {
                    gen.Emit(OpCodes.Ldloc, local);
                    gen.Emit(OpCodes.Ldc_I4, i);
                    act();
                    gen.Emit(OpCodes.Stelem_Ref);
                };

            //BaseContainer4Surrogacy container, 
            if (canSeeContainer)
            {
                emitAdd(0,
                    () =>
                    {
                        gen.Emit(OpCodes.Ldarg_0);
                        gen.EmitCall(strategy.ContainerProperty.GetBuilder().GetGetMethod());
                    });
            }

            ////object bag, 
            if (canSeeStateBag)
            {
                emitAdd(canSeeContainer ? 1 : 0,
                    () =>
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
                () => gen.Emit(OpCodes.Ldloc, interceptor.Locals["S_Method"]));

            //object[] args
            emitAdd(
                offset++,
                () => gen.Emit(OpCodes.Ldloc, interceptor.Locals["Args"]));

            return local;
        }
    }
}