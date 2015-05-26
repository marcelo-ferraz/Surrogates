using Surrogates.Applications.Contracts;
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
            public object ValidateBeforeExecute(string s_name, Delegate s_method, object[] s_args, Dictionary<string, Action<object[]>> s_PreValidators)
            {
                s_PreValidators[s_name](s_args);

                return s_method.DynamicInvoke(s_args);
            }
        }

        private static AndExpression<T> AddAllValidators<T>(this ApplyExpression<T> that, IPreValidator[] validations, IEnumerable<MethodInfo> ms)
        {
            var ext =
                new ShallowExtension<T>();

            Pass.On(that, to: ext);

            var validators = validations
                .SelectMany(ass => 
                    ((Assert.List4.Parameters)ass).Validators);

            var preValidators =
                new Dictionary<string, Action<object[]>>();

            AndExpression<T> expr =  ext
                .Factory
                .AddProperty<Dictionary<string, Action<object[]>>>("PreValidators", preValidators);

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

                expr = expr
                    .And
                    .Replace
                    .Method(method.Name)
                    .Using<ValidatorInterceptor<T>>("ValidateBeforeExecute");
            }

            return expr;
        }

        private static Action<object[]> GetPreValidator4ThisMethod(MethodInfo method, IEnumerable<Assert.Entry4.Parameters> validators)
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

        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, Func<T, Delegate>[] methods, params IPreValidator[] pre)
        {
            var obj = (T)FormatterServices
                .GetUninitializedObject(typeof(T));

            return that.AddAllValidators<T>(                
                pre,
                methods.Select(m => m(obj).Method));
        }

        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, Func<T, Delegate> method, params IPreValidator[] pre)
        {
            var obj = (T)FormatterServices
                .GetUninitializedObject(typeof(T));

            return that.AddAllValidators<T>(                
                pre,
                new[] { method(obj).Method });
        }

        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, string[] methods, params IPreValidator[] pre)
        {
            return that.AddAllValidators<T>(
                pre,
                methods.Select(m => typeof(T).GetMethod4Surrogacy(m)));
        }

        public static AndExpression<T> Contracts<T>(
            this ApplyExpression<T> that, string method, params IPreValidator[] pre)
        {
            return that.AddAllValidators<T>(
                pre,
                new[] { typeof(T).GetMethod4Surrogacy(method) });
        }
    }
}
