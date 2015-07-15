using Surrogates.Applications.Contracts;
using Surrogates.Applications.Contracts.Collections;
using Surrogates.Applications.Contracts.Model;
using Surrogates.Applications.Model;
using Surrogates.Expressions;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Surrogates.Applications
{
    public static class ContractsMixins
    {
        private static AndExpression<T> AddAllPreValidators<T>(this ApplyExpression<T> that, IParamValidator[] validations, IEnumerable<MethodInfo> ms)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On(that, to: ext);

            var validators = validations
                .SelectMany(ass => 
                    ((AssertionList4Parameters)ass).Validators);

            var preValidators =
                new Dictionary<string, Action<object[]>>();

            AndExpression<T> expr = null;

            foreach (var method in ms)
            {
                var preValidator =
                        GetPreValidator4ThisMethod(method, validators);

                if (preValidator == null) { continue; }

                if (preValidators.ContainsKey(method.Name))
                {
                    preValidators[method.Name] = (Action<object[]>)
                        Delegate.Combine(preValidators[method.Name], preValidator);
                }
                else 
                { preValidators.Add(method.Name, preValidator); }

                expr = (expr != null ? expr.And : ext.Factory)
                    .Replace
                    .Method(method.Name)
                    .Using<ContractsInterceptor<T>>("ValidateBeforeExecute");
            }
            
            return expr.And
                .AddProperty<Dictionary<string, Action<object[]>>>("PreValidators", preValidators);
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
        /// <param name="validations">The verifications. Make use of <seealso cref="Surrogates.Applications.Contracts.Presume"/></param>
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
        /// <param name="validations">The verifications. Make use of <seealso cref="Surrogates.Applications.Contracts.Presume"/></param>
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
        /// <param name="validations">The verifications. Make use of <seealso cref="Surrogates.Applications.Contracts.Presume"/></param>
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
