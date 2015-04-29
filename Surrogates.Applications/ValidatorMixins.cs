using Surrogates.Applications.Validators;
using Surrogates.Expressions;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Applications
{
    public static class ValidatorMixins
    {
        public class ValidatorInterceptor<T>
        {
            public object ValidateBeforeExecute(string s_name, Delegate s_method, object[] args, IDictionary<string, Action<object[]>> s_Validators)
            {
                s_Validators[s_name](args);

                return s_method.DynamicInvoke(args);
            }
        }

        public static AndExpression<T> Validators<T>(
            this ApplyExpression<T> that, Func<T, Delegate>[] methods, params IParamValidators[] assertions)
        {
            var ext = 
                new ShallowExtension<T>();

            Pass.On(that, to: ext);

            //var val = new ParametersValidator<T>();

            var obj = (T) FormatterServices
                .GetUninitializedObject(typeof(T));

            AndExpression<T> expr = null;

            foreach(var method in methods)
            {
                var @params = 
                    method(obj).Method.GetParameters();

                foreach(var arg in @params)
                {
                    for (int i = 0; i < assertions.Length; i++)
                    {
                        var assertion =
                            assertions[i] as Assert.List4.Parameters;

                        foreach(var validator in assertion.Validators)
                        {
                            if (arg.Name != validator.Name)
                            { continue; }

                            expr =
                                (expr != null ? expr.And : ext.Factory)
                                .Replace
                                .This(method)
                                .Using<ValidatorInterceptor<T>>("ValidateBeforeExecute");
                        }                        
                    }
                }
            }
            //AndExpression<T> expr = null;

            //for (int i = 0; i < assertions.Length; i++)
            //{
            //    for (int j = 0; j < assertions[i].Validators.Count; j++)
            //    {
            //        expr =
            //            (expr != null ? expr.And : ext.Factory)
            //            .Replace
            //            .Method(null)
            //            .Using<ValidatorInterceptor<T>>("ValidateBeforeExecute");
            //    }
            //}

            //return expr.And.AddProperty("Validators", val.Validators);
            return null;
        }
    }
}
