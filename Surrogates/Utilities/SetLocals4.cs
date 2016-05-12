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
    public static class SetLocals4
    {
        private static Type _typeOfGet_Item = typeof(Dictionary<string, Func<object, Delegate>>);
        private static Type _typeOfInvoke = typeof(Func<object, Delegate>);

        public static LocalBuilder AllComplexParameters(Strategy strat, Strategy.InterceptorInfo interceptor, MethodInfo baseMethod, OverridenMethod overriden)
        {
            LocalBuilder returnField = baseMethod.ReturnType != TypeOf.Void ?
                overriden.Generator.DeclareLocal(baseMethod.ReturnType) :
                null;

            Func<string, bool> has =
                overriden.Locals.ContainsKey;

            foreach (var param in interceptor.Method.GetParameters())
            {
                if (param.Is4SelfArguments() && !has("Args"))
                {
                    overriden.Locals.Add("Args", 
                        SetLocals4.ArgsParam(overriden.Generator, param, baseMethod.GetParameters()));
                }

                else if (param.Is4SelfMethod(baseMethod) && !has("S_Method"))
                {
                    overriden.Locals.Add("S_Method",
                        SetLocals4.OriginalMethodAsParameter(overriden.Generator, baseMethod, param, strat.BaseMethods.Field));
                }

                else if (param.IsDynamic_() && !has("ThisDynamic_"))
                {
                    overriden.Locals.Add("ThisDynamic_",
                        SetLocals4.ThisDynamic_(overriden, strat, baseMethod, param));
                }

                else 
                {
                    var key = string.Concat(
                        param.Name, "+", param.ParameterType.Name);

                    if (param.Is4SomeMethod() && !has(key))
                    {
                        overriden.Locals.Add(key,
                            SetLocals4.MethodAsParameter(overriden.Generator, baseMethod, param, strat.BaseMethods.Field));
                    }
                }
            }
            
            return returnField;
        }

        public static LocalBuilder AllComplexParameters(Strategy.ForMethods strat, MethodInfo baseMethod, OverridenMethod overriden)
        {
            return SetLocals4.AllComplexParameters(
                strat, strat.Interceptor, baseMethod, overriden);
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
            var local = gen.DeclareLocal(TypeOf.ObjectArray);

            var arguments =
                gen.DeclareLocal(TypeOf.ObjectArray);

            gen.Emit(OpCodes.Ldc_I4, baseParams.Length);
            gen.Emit(OpCodes.Newarr, TypeOf.Object);
            gen.Emit(OpCodes.Stloc, local);

            if (baseParams.Length < 1) { return local; }
            
            for (int i = 0; i < baseParams.Length; i++)
            {
                gen.Emit(OpCodes.Ldloc, local);
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
            return SetLocals4.MethodAsParameter(gen, baseMethod, param, baseMethodsField, is4Dynamic_);
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

            var local = gen.DeclareLocal(isDynamic_ ? TypeOf.Delegate : param.ParameterType);

            var isFunc =
                baseMethod.ReturnType != TypeOf.Void;

            Type delType =
                Infer.DelegateTypeFrom(baseMethod);

            gen.Emit(OpCodes.Ldsfld, baseMethodsField);
            gen.Emit(OpCodes.Ldstr, baseMethod.Name);
            gen.EmitCall(_typeOfGet_Item.GetMethod4Surrogacy("get_Item"));
            gen.Emit(OpCodes.Ldarg_0);
            gen.EmitCall(_typeOfInvoke.GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance), new[] { TypeOf.Object });

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

            if (param.ParameterType == TypeOf.Delegate)
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

                if (genDef == TypeOf.Func)
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
                SetLocals4.MethodAsParameter(gen, method, param, baseMethodsField) :
                null;
        }

        /// <summary>
        /// Creates the local for this dynamic underscore and sets its all values
        /// </summary>
        /// <param name="gen"></param>
        /// <param name="strategy"></param>
        /// <param name="overriden"></param>
        /// <param name="originalMethod"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        internal static LocalBuilder ThisDynamic_(OverridenMethod overriden, Strategy strategy, MethodInfo originalMethod, ParameterInfo param)
        {
            var gen = overriden.Generator;

            if (!overriden.Locals.ContainsKey("S_Method"))
            { 
                overriden.Locals.Add("S_Method", 
                    SetLocals4.OriginalMethodAsParameter(gen, originalMethod, param, strategy.BaseMethods.Field, is4Dynamic_: true)); 
            }

            if (!overriden.Locals.ContainsKey("Args"))
            {
                overriden.Locals.Add("Args", 
                    SetLocals4.ArgsParam(gen, param, originalMethod.GetParameters()));
            }

            var local =
                gen.DeclareLocal(TypeOf.ObjectArray);

            bool canSeeContainer =
                   strategy.Accesses.HasFlag(Access.Container);
            bool canSeeStateBag =
                strategy.Accesses.HasFlag(Access.StateBag);
            bool canSeeInstance =
                strategy.Accesses.HasFlag(Access.Instance);

            int offset = (canSeeInstance ? 1 : 0) + (canSeeContainer ? 1 : 0) + (canSeeStateBag ? 1 : 0);

            gen.Emit(OpCodes.Ldc_I4, 4 + offset);
            gen.Emit(OpCodes.Newarr, TypeOf.Object);
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
                () => gen.Emit(OpCodes.Ldloc, overriden.Locals["S_Method"]));

            //object[] args
            emitAdd(
                offset++,
                () => gen.Emit(OpCodes.Ldloc, overriden.Locals["Args"]));

            return local;
        }
    }
}