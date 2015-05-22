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
        private static IParamValidators AddValidator(IParamValidators assertions, string[] @params, Func<int, ParameterInfo[], Action<object[]>> validator)
        {
            var ass = (Assert.List4.Parameters)
                (assertions ?? (assertions = new Assert.List4.Parameters()));
            

            for (int i = 0; i < @params.Length; i++)
            {
                ass.Validators
                    .Add(new Assert.Entry4.Parameters
                    {
                        ParameterName = @params[i],
                        Action = validator
                    });
            }

            return assertions;
        }

        public static void Throw(string format, params object[] args)
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
                    var name = p[i].Name;
                    var type = p[i].ParameterType;

                    if(p[i].ParameterType == typeof(string))
                    {
                        return args =>
                        {
                            if (BaseValidators.ValidateRequiredString(args[i]))
                            { Throw("The given value for '{0}' is not the default of {1}!", name, type); }
                        };
                    }

                    return args =>
                    {
                        if (BaseValidators.ValidateRequired(args[i], type))
                        { Throw("The given value for '{0}' is not the default of {1}!", name, type); }
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
                    var name = p[i].Name;
                    var type = p[i].ParameterType;

                    if (type == typeof(string))
                    {
                        return args =>
                        {
                            if (!BaseValidators.ValidateRequiredString(args[i]))
                            { Throw("The given value for '{0}' is required!", name); }
                        };
                    }

                    return args =>
                    {
                        if (!BaseValidators.ValidateRequired(args[i], type))
                        { Throw("The given value for '{0}' is required!", name); }
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
                (i, p) => {

                    var name = p[i].Name;
                    
                    return 
                        args => 
                        {
                            if(!BaseValidators.ValidateInBetween<P>(min, max, (P)args[i]))
                            { Throw("The given value for '{0}' is not in between of {1} and {2}!", name, min, max); }                        
                        };
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
                        var name = p[i].Name;
                        
                        if(!BaseValidators.ValidateBiggerThan<P>(higher, (P)args[i]))
                        { Throw("The given value for '{0}' is not bigger than {1}!", name, higher); }                        
                    });
        }

        public static IParamValidators LowerThan<P>(this IParamValidators self, P lower, params string[] @params)
            where P : struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) => {
                    var name = p[i].Name;

                    return args =>
                    {
                        if (!BaseValidators.ValidateLowerThan<P>(lower, (P)args[i]))
                        { Throw("The given value for '{0}' is not lower than {1}!", name, lower); }
                    };
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
                (i, p) =>{
                    var name = p[i].Name;
                    
                    return args => {
                        if(!BaseValidators.ValidateRegex(expr, (string)args[i]))
                        { Throw("The given value for '{0}' does not match the expression '{1}'!", name, expr.ToString()); }                        
                    };
                });                   
        }

        public static IParamValidators Composite<T>(this IParamValidators self, string param, params IPropValidators[] validators)
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

        public static IParamValidators Composite(this IParamValidators self, params Delegate[] preValidators)
        {
            var ass = (Assert.List4.Parameters)
                (self ?? (self = new Assert.List4.Parameters()));
            
            foreach (var preValidator in preValidators)
            {
                var @params = 
                    preValidator.Method.GetParameters();

                if(@params.Length < 1) { continue; }

                AddValidator(
                    self,
                    new [] { @params[0].Name },
                    (j, p) => 
                    {
                        var indexes = new List<int>();
                        foreach (var valParam in @params)
                        {
                            int index = -1;
                            foreach (var baseParam in p)
                            {
                                index++;                             
                                if (baseParam.Name != valParam.Name) { continue; }
                                if (!baseParam.ParameterType.IsAssignableFrom(valParam.ParameterType)) { continue; }

                                indexes.Add(index);
                            }
                        }
                        
                        return a =>
                        {
                            var args = (object[])
                                Array.CreateInstance(typeof(object), indexes.Count);
                            
                            for (int i = 0; i < indexes.Count; i++)
                            { args[i] = a[indexes[i]]; }

                            var result = 
                                preValidator.DynamicInvoke(args);

                            if (result == null || (result is bool && !(bool)result))
                            { 
                                var sb = new StringBuilder();

                                sb.Append("The values for the parameters [ ");

                                for (int i = 0; i < p.Length; i++)
                                {                                    
                                    sb.AppendFormat(" '{0}'{1} ", p[i].Name, (i == p.Length - 1) ? "" : ",");
                                }

                                sb.Append("] did not match the composite pre validator!");

                                Throw(sb.ToString());
                            }
                        };
                    });
            }

            return self;
        }
    }
}