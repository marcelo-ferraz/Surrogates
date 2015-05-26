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
        private static IPreValidator AddValidator(IPreValidator assertions, string[] @params, Func<int, ParameterInfo[], Action<object[]>> validator)
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

        public static IPreValidator Null(this IPreValidator self, params string[] @params)
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


        public static IPreValidator Required(this IPreValidator self, params string[] @params)
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

        public static IPreValidator Email(this IPreValidator self, params string[] @params)
        {
            return Regex(self, BaseValidators.EmailRegexpr, @params);
        }

        public static IPreValidator Url(this IPreValidator self, params string[] @params)
        {
            return Regex(self, BaseValidators.UrlRegexpr, @params);
        }

        public static IPreValidator Number(this IPreValidator self, params string[] @params)
        {
            return Regex(self, BaseValidators.IsNumberRegexpr, @params);
        }

        public static IPreValidator InBetween<P>(this IPreValidator self, P min, P max, params string[] @params)
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

        public static IPreValidator BiggerThan<P>(this IPreValidator self, P higher, params string[] @params)
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

        public static IPreValidator LowerThan<P>(this IPreValidator self, P lower, params string[] @params)
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

        public static IPreValidator Regex(this IPreValidator self, string expr, params string[] @params)
        {
            return Regex(self, new Regex(expr), @params);
        }

        public static IPreValidator Regex(this IPreValidator self, Regex expr, params string[] @params)
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

        public static IPreValidator Composite<T>(this IPreValidator self, string param, params IPropValidators[] validators)
        { 
            return Composite<T>(self, new string[] { param }, validators);
        }

        public static IPreValidator Composite<T>(this IPreValidator self, string[] @params, params IPropValidators[] validators)
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

        public static IPreValidator Composite(this IPreValidator self, params Delegate[] preValidators)
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