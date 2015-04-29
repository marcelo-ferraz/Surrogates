using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Surrogates.Utilities.Mixins;
using System.Runtime.Serialization;
using System.Reflection;
using System.Linq.Expressions;

namespace Surrogates.Applications.Validators
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
                        Name = @params[i],
                        Validation = validator
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

    public static class Params
    {
        public class Validators
        {
            public IParamValidators Required(params string[] @params)
            {
                return ParameterAssertionMixins.Required(null, @params);
            }

            public IParamValidators Email(params string[] @params)
            {
                return ParameterAssertionMixins.Email(null, @params);
            }

            public IParamValidators Url(params string[] @params)
            {
                return ParameterAssertionMixins.Url(null, @params);
            }

            public IParamValidators Number(params string[] @params)
            {
                return ParameterAssertionMixins.Number(null, @params);
            }

            public IParamValidators InBetween<P>(P min, P max, params string[] @params)
                where P : struct
            {
                return ParameterAssertionMixins.InBetween(null, min, max, @params);
            }

            public IParamValidators BiggerThan<P>(P higher, params string[] @params)
                where P : struct
            {
                return ParameterAssertionMixins.BiggerThan(null, higher, @params);
            }

            public IParamValidators LowerThan<P>(P lower, params string[] @params)
                where P : struct
            {
                return ParameterAssertionMixins.LowerThan<P>(null, lower, @params);
            }

            public IParamValidators Regex(string expr, params string[] @params)
            {
                return ParameterAssertionMixins.Regex(null, expr, @params);
            }

            public IParamValidators Regex(Regex expr, params string[] @params)
            {
                return ParameterAssertionMixins.Regex(null, expr, @params);
            }

            //public Assertion.ListFor.Parameters Complex<T>(string param, params PropertiesValidator<T>[] validators)
            //{
            //    return null;
            //}
        }
         
        public static Validators Are { get; set; }

        static Params()
        {
            Are = new Params.Validators();             
        }
    }
}