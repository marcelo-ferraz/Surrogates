﻿using Surrogates.Aspects.Contracts;
using Surrogates.Aspects.Contracts.Collections;
using Surrogates.Aspects.Contracts.Model;
using Surrogates.Aspects.Model;
using Surrogates.Expressions;
using Surrogates.Tactics;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Surrogates.Aspects.Utilities;

namespace Surrogates.Aspects
{
    public static class ContractsMixins
    {
        private static AndExpression<T> AddAllPreValidators<T>(this ApplyExpression<T> that, IParamValidator[] validations, IEnumerable<MethodInfo> ms)
        {
            var ext =
                new ShallowExtension<T>();

            InternalsInspector.GetInternals(that, to: ext);

            var validators = validations
                .SelectMany(ass => 
                    ((AssertionList4Parameters)ass).Validators);

            var preValidators =
                new Dictionary<IntPtr, Action<object[]>>();

            AndExpression<T> expr = null;

            foreach (var method in ms)
            {
                var preValidator =
                        GetPreValidator4ThisMethod(method, validators);

                if (preValidator == null) { continue; }

                var handle = 
                    method.MethodHandle.Value;

                if (preValidators.ContainsKey(method.MethodHandle.Value))
                {
                    preValidators[handle] = 
                        (Action<object[]>)
                        Delegate.Combine(preValidators[handle], preValidator);
                }
                else
                { preValidators.Add(handle, preValidator); }

                expr = (expr != null ? expr.And : ext.Factory)
                    .Replace
                    .Method(method.Name)
                    .Using<ContractsInterceptor<T>>("ValidateBeforeExecute");
            }

            preValidators = ext
                .Strategies
                .MergeProperty("PreValidators", preValidators);
            
            return expr
                .And
                .AddProperty<Dictionary<IntPtr, Action<object[]>>>("PreValidators", preValidators);
        }

        private static Action<object[]> GetPreValidator4ThisMethod(MethodInfo method, IEnumerable<AssertionEntry4Parameters> validators)
        {
            Action<object[]> preValidator = null;

            var @params =
                method.GetParameters();

            foreach (var validator in validators)
            {
                Action<object[]> paramValidate;

                for (var i = 0; i < @params.Length; i++)
                {
                    if (@params[i].Name != validator.ParameterName) { continue; }

                    paramValidate =
                        validator.Action(i, @params);

                    preValidator = (Action<object[]>)(preValidator != null ?
                        Delegate.Combine(preValidator, paramValidate) :
                        paramValidate);                    
                }
            }

            return preValidator;
        }

        /// <summary>
        /// Applies verifications to a prop's call
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The Expression</param>
        /// <param name="that">The Expression</param>
        /// <param name="properties">The properties where those verification shall be applied</param>
        /// <returns></returns>
        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, Func<T, Delegate>[] methods, params IParamValidator[] validations)
        {
            var obj = (T)FormatterServices
                .GetUninitializedObject(typeof(T));

            return that.AddAllPreValidators<T>(                
                validations,
                methods.Select(m => m(obj).Method));
        }

        /// <summary>
        /// Applies verifications to a prop's call
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The Expression</param>
        /// <param name="prop">The prop where those verification shall be applied</param>
        /// <param name="validations">The verifications. Make use of <seealso cref="Surrogates.Aspects.Contracts.Presume"/></param>
        /// <returns>Returns the expression</returns>
        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, Func<T, Delegate> method, params IParamValidator[] validations)
        {
            var obj = (T)FormatterServices
                .GetUninitializedObject(typeof(T));

            return that.AddAllPreValidators<T>(                
                validations,
                new[] { method(obj).Method });
        }

        /// <summary>
        /// Applies verifications to a prop's call
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The Expression</param>
        /// <param name="properties">The properties where those verification shall be applied</param>
        /// <param name="validations">The verifications. Make use of <seealso cref="Surrogates.Aspects.Contracts.Presume"/></param>
        /// <returns></returns>
        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, string[] methods, params IParamValidator[] preValidations)
        {
            return that.AddAllPreValidators<T>(
                preValidations,
                methods.Select(m => typeof(T).GetMethod4Surrogacy(m)));
        }

        /// <summary>
        /// Applies verifications to a prop's call
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The Expression</param>
        /// <param name="prop">The prop where those verification shall be applied</param>
        /// <param name="validations">The verifications. Make use of <seealso cref="Surrogates.Aspects.Contracts.Presume"/></param>
        /// <returns></returns>
        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, string method, params IParamValidator[] preValidations)
        {
            return that.AddAllPreValidators<T>(
                preValidations,
                new[] { typeof(T).GetMethod4Surrogacy(method) });
        }
    }
}
