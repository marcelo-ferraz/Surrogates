using Surrogates.Applications.Contracts;
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

        private static void Throw(string format, params object[] args)
        { 
            throw new ArgumentException(
                string.Format(format, args)); 
        }

        public static IParamValidators Null(this IParamValidators self, params string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) => {
                    if(p.ParameterType == typeof(string))
                    {
                        return args =>
                        {
                            if (BaseValidators.ValidateRequiredString(args[i]))
                            { Throw("The given value for '{0}' is not the default of {1}!", p.Name, p.ParameterType); }
                        };
                    }

                    return args =>
                    {
                        if (BaseValidators.ValidateRequired(args[i], p.ParameterType))
                        { Throw("The given value for '{0}' is not the default of {1}!", p.Name, p.ParameterType); }
                    };
                });
        }


        public static IParamValidators Required(this IParamValidators self, params string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    if (p.ParameterType == typeof(string))
                    {
                        return args =>
                        {
                            if (!BaseValidators.ValidateRequiredString(args[i]))
                            { Throw("The given value for '{0}' is required!", p.Name); }
                        };
                    }

                    return args =>
                    {
                        if (!BaseValidators.ValidateRequired(args[i], p.ParameterType))
                        { Throw("The given value for '{0}' is required!", p.Name); }
                    };
                });
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
                    args => {
                        if(!BaseValidators.ValidateInBetween<P>(min, max, (P)args[i]))
                        { Throw("The given value for '{0}' is not in between of {1} and {2}!", p.Name, min, max); }                        
                    });
        }

        public static IParamValidators BiggerThan<P>(this IParamValidators self, P higher, params string[] @params)
            where P : struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => {
                        if(!BaseValidators.ValidateBiggerThan<P>(higher, (P)args[i]))
                        { Throw("The given value for '{0}' is not bigger than {1}!", p.Name, higher); }                        
                    });
        }

        public static IParamValidators LowerThan<P>(this IParamValidators self, P lower, params string[] @params)
            where P : struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => {
                        if(!BaseValidators.ValidateLowerThan<P>(lower, (P)args[i]))
                        { Throw("The given value for '{0}' is not lower than {1}!", p.Name, lower); }                        
                    });                    
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
                    args => {
                        if(!BaseValidators.ValidateRegex(expr, (string)args[i]))
                        { Throw("The given value for '{0}' does not match the expression '{1}'!", p.Name, expr.ToString()); }                        
                    });                   
        }

        public static IParamValidators ComplexObject<T>(this IParamValidators self, string param, params IPropValidators[] validators)
        { 
            return Composite<T>(self, new string[] { param }, validators);
        }

        public static IParamValidators Composite<T>(this IParamValidators self, string[] @params, params IPropValidators[] validators)
        {
            Func<T, bool> assertion = null;

            foreach (var validator in validators.SelectMany(v => ((Assert.List4.Properties)v).Validators))                
            {
                assertion = assertion != null ?
                    (arg => assertion(arg) && (bool)validator.Validation.DynamicInvoke(arg)) :
                    (Func<T, bool>)validator.Validation;
            }

            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => assertion((T)args[i]));
        }
    }
}