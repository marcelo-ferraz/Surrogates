using Surrogates.Applications.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Surrogates.Applications
{
    public static class ParameterAssertionMixins
    {
        private static IParamValidators AddValidator(IParamValidators assertions, string[] @params, Func<int, ParameterInfo, Action<object[]>> validator)
        {
            for (int i = 0; i < @params.Length; i++)
            {
                ((Assert.List4.Parameters)(assertions ?? (assertions = new Assert.List4.Parameters())))
                    .Validators
                    .Add(new Assert.Entry4.Parameters
                    {
                        ParameterName = @params[i],
                        Action = validator
                    });
            }

            return assertions;
        }

        public static IParamValidators Required(this IParamValidators self, params string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) => p.ParameterType == typeof(string) ?
                    (Action<object[]>)(args => BaseValidators.ValidateRequiredString(args[i])) :
                    args => BaseValidators.ValidateRequired(args[i], args[i].GetType()));
        }

        public static IParamValidators Email(this IParamValidators self, params string[] @params)
        {
            return Regex(self, BaseValidators.EmailRegexpr, @params);
        }

        public static IParamValidators Url(this IParamValidators self, params string[] @params)
        {
            return Regex(self, BaseValidators.UrlRegexpr, @params);
        }

        public static IParamValidators Number(this IParamValidators self, params string[] @params)
        {
            return Regex(self, BaseValidators.IsNumberRegexpr, @params);
        }

        public static IParamValidators InBetween<P>(this IParamValidators self, P min, P max, params string[] @params)
            where P : struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => BaseValidators.ValidateInBetween<P>(min, max, (P)args[i]));
        }

        public static IParamValidators BiggerThan<P>(this IParamValidators self, P higher, params string[] @params)
            where P : struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => BaseValidators.ValidateBiggerThan<P>(higher, (P)args[i]));
        }

        public static IParamValidators LowerThan<P>(this IParamValidators self, P lower, params string[] @params)
            where P : struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => BaseValidators.ValidateLowerThan<P>(lower, (P)args[i]));
        }

        public static IParamValidators Regex(this IParamValidators self, string expr, params string[] @params)
        {
            return Regex(self, new Regex(expr), @params);
        }

        public static IParamValidators Regex(this IParamValidators self, Regex expr, params string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => BaseValidators.ValidateRegex(expr, (string)args[i]));
        }

        public static IParamValidators Complex<T>(this IParamValidators self, string[] @params, IPropValidators validators)
        {
            Delegate assertion = null;

            foreach (var validator in ((Assert.List4.Properties)validators).Validators)
            {
                assertion = assertion != null ?
                    Delegate.Combine(assertion, validator.Validation) :
                    validator.Validation;
            }

            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => assertion.DynamicInvoke((T)args[i]));
        }
    }

}
