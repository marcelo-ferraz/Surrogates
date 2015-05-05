using Surrogates.Applications.Validators;
using Surrogates.Expressions;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Applications
{
    public static class ContractsMixins
    {
        public class ValidatorInterceptor<T>
        {
            public object ValidateBeforeExecute(string s_name, Delegate s_method, object[] args, IDictionary<string, Action<object[]>> s_PreValidators)
            {
                s_PreValidators[s_name](args);

                return s_method.DynamicInvoke(args);
            }
        }

        private static AndExpression<T> AddValidators<T>(this ApplyExpression<T> that, IParamValidators[] validations, IEnumerable<MethodInfo> ms)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On(that, to: ext);

            var validators = validations
                .SelectMany(ass => 
                    ((Assert.List4.Parameters)ass).Validators);

            var preValidators =
                new Dictionary<string, Action<object[]>>();

            AndExpression<T> expr = null;

            foreach (var method in ms)
            {
                var preValidator =
                        GetPreValidators(method, validators);

                if (preValidators.ContainsKey(method.Name))
                {
                    preValidators[method.Name] = (Action<object[]>)(preValidators[method.Name] != null ?
                            Delegate.Combine(preValidators[method.Name], preValidator) :
                            preValidator);
                }
                else 
                { preValidators.Add(method.Name, preValidator); }

                expr = (expr != null ? expr.And : ext.Factory)
                    .Replace
                    .Method(method.Name)
                    .Using<ValidatorInterceptor<T>>("ValidateBeforeExecute");
            }

            return expr.And
                .AddProperty<Dictionary<string, Action<object[]>>>("PreValidators", preValidators);
        }

        private static Action<object[]> GetPreValidators(MethodInfo method, IEnumerable<Assert.Entry4.Parameters> validators)
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
                        validator.Action(i, @params[i]);

                    preValidator = (Action<object[]>)(preValidator != null ?
                        Delegate.Combine(preValidator, paramValidate) :
                        paramValidate);
                }
            }

            return preValidator;
        }

        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, Func<T, Delegate>[] methods, params IParamValidators[] validations)
        {
            var obj = (T)FormatterServices
                .GetUninitializedObject(typeof(T));

            return that.AddValidators<T>(                
                validations,
                methods.Select(m => m(obj).Method));
        }

        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, Func<T, Delegate> method, params IParamValidators[] validations)
        {
            var obj = (T)FormatterServices
                .GetUninitializedObject(typeof(T));

            return that.AddValidators<T>(                
                validations,
                new[] { method(obj).Method });
        }

        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, string[] methods, params IParamValidators[] validations)
        {
            return that.AddValidators<T>(                
                validations,
                methods.Select(m => typeof(T).GetMethod4Surrogacy(m)));
        }

        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, string method, params IParamValidators[] validations)
        {
            return that.AddValidators<T>(                
                validations,
                new[] { typeof(T).GetMethod4Surrogacy(method) });
        }
    }
}
