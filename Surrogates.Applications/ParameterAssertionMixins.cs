using Surrogates.Applications.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Surrogates.Applications.Mixins;
using System.Collections;
namespace Surrogates.Applications
{
    public static class ParameterAssertionMixins
    {
        private static IParamValidators AddValidator(IParamValidators assertions, string[] @params, Func<int, ParameterInfo[], Action<object[]>> validator)
        {
            var ass = (Assert_.List4.Parameters)
                (assertions ?? (assertions = new Assert_.List4.Parameters()));
            

            for (int i = 0; i < @params.Length; i++)
            {
                ass.Validators
                    .Add(new Assert_.Entry4.Parameters
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

        public static IParamValidators IsNullOrDefault(this IParamValidators self, params string[] @params)
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
                            if (!string.IsNullOrEmpty((string)args[i]))
                            { Throw("The given value for '{0}' is not the default of {1}!", name, type); }
                        };
                    }

                    return args =>
                    {
                        if (!_Validate.IsNotNullOrDefault(args[i], type))
                        { Throw("The given value for '{0}' is not the default of {1}!", name, type); }
                    };
                });
        }
        
        public static IParamValidators IsNotNullOrDefault(this IParamValidators self, params string[] @params)
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
                            if (string.IsNullOrEmpty((string)(args[i])))
                            { Throw("The given value for '{0}' is required!", name); }
                        };
                    }

                    return args =>
                    {
                        if (!_Validate.IsNotNullOrDefault(args[i], type))
                        { Throw("The given value for '{0}' is required!", name); }
                    };
                });
        }

        public static IParamValidators IsAnEmail(this IParamValidators self, params string[] @params)
        {
            return ThisRegex(self, _Validate.EmailRegexpr, @params);
        }

        public static IParamValidators IsAnUrl(this IParamValidators self, params string[] @params)
        {
            return ThisRegex(self, _Validate.UrlRegexpr, @params);
        }

        public static IParamValidators IsNumber(this IParamValidators self, params string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name = p[i].Name;
                    var expr = _Validate.IsNumberRegexpr;
                    return args =>
                    {
                        if (!_Validate.Regex(expr, (string)args[i]))
                        { Throw("The given value for '{0}' is not a number!", name); }
                    };
                }); 
        }

        internal static IParamValidators IsNaN(this IParamValidators self, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name = p[i].Name;
                    var expr = _Validate.IsNumberRegexpr;
                    return args =>
                    {
                        if (_Validate.Regex(expr, (string)args[i]))
                        { Throw("The given value for '{0}' is a number!", name); }
                    };
                });  
        }
        
        public static IParamValidators IsInBetween<P>(this IParamValidators self, P min, P max, params string[] @params)
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
                            if(!_Validate.InBetween<P>(min, max, (P)args[i]))
                            { Throw("The given value for '{0}' is not in between of {1} and {2}!", name, min, max); }                        
                        };
                });
        }
        
        public static IParamValidators Greater<P>(this IParamValidators self, P higher, params string[] @params)
            where P : struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                    args => {
                        var name = p[i].Name;
                        
                        if(!_Validate.Greater<P>(higher, (P)args[i]))
                        { Throw("The given value for '{0}' is not bigger than {1}!", name, higher); }                        
                    });
        }

        public static IParamValidators IsLowerThan<P>(this IParamValidators self, P lower, params string[] @params)
            where P : struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) => {
                    var name = p[i].Name;

                    return args =>
                    {
                        if (!_Validate.Less<P>(lower, (P)args[i]))
                        { Throw("The given value for '{0}' is not lower than {1}!", name, lower); }
                    };
                });                    
        }

        public static IParamValidators ThisRegex(this IParamValidators self, string expr, params string[] @params)
        {
            return ThisRegex(self, new Regex(expr), @params);
        }

        public static IParamValidators ThisRegex(this IParamValidators self, Regex expr, params string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>{
                    var name = p[i].Name;
                    
                    return args => {
                        if(!_Validate.Regex(expr, (string)args[i]))
                        { Throw("The given value for '{0}' does not match the expression '{1}'!", name, expr.ToString()); }                        
                    };
                });                   
        }

        public static IParamValidators That<T>(this IParamValidators self, string param, params IPropValidators[] validators)
        { 
            return That<T>(self, new string[] { param }, validators);
        }

        public static IParamValidators That<T>(this IParamValidators self, string[] @params, params IPropValidators[] validators)
        {
            Func<T, bool> assertion = null;

            foreach (var validator in validators.SelectMany(v => ((Assert_.List4.Properties)v).Validators))                
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

        public static IParamValidators That(this IParamValidators self, params Delegate[] preValidators)
        {
            var ass = (Assert_.List4.Parameters)
                (self ?? (self = new Assert_.List4.Parameters()));
            
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

        internal static IParamValidators AreEqual(this IParamValidators self, object expected, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name = p[i].Name;

                    return args =>
                    {
                        if (!object.Equals(expected, args[i]))
                        { Throw("The given value for '{0}', {1} is not equals to {2}!", name, args[i], expected); }
                    };
                });  
        }

        internal static IParamValidators AreReferenceEqual(this IParamValidators self, object expected, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name = p[i].Name;

                    return args =>
                    {
                        if (!object.ReferenceEquals(expected, args[i]))
                        { Throw("The given value for '{0}', {1} is not equals to {2}!", name, args[i], expected); }
                    };
                });  
        }
        
        internal static IParamValidators Contains(this IParamValidators self, object expected, string[] @params)
        {
            return AddValidator(
               self,
               @params,
               (i, p) =>
               {
                   var contains =
                       _Validate.Contains(i, p, expected);

                   var name = p[i].Name;

                   return args =>
                   {
                       if (contains(args))
                       { Throw("The supplied value '{0}', was not found on the given value of '{1}'!", expected, name); }
                   };
               });
        }

        internal static IParamValidators DoesNotContains(this IParamValidators self, object expected, string[] @params)
        {
            return AddValidator(
               self,
               @params,
               (i, p) =>
               {
                   var contains =
                       _Validate.Contains(i, p, expected);

                   var name = p[i].Name;

                   return args =>
                   {
                       if (!contains(args))
                       { Throw("The supplied value '{0}', was not found on the given value of '{1}'!", expected, name); }
                   };
               });
        }

        internal static IParamValidators IsAssignableFrom<T>(this IParamValidators self, string[] @params)
        {
            return self.IsAssignableFrom(typeof(T), @params);
        }

        internal static IParamValidators IsAssignableFrom(this IParamValidators self, Type expectedType, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name = p[i].Name;

                    return args =>
                    {
                        if (args[i].GetType().IsAssignableFrom(expectedType))
                        { Throw("The type, '{0}', of the parameter '{1}' is not assignable from {2} !", args[i].GetType(), name, expectedType); }
                    };
                });  
        }

        internal static IParamValidators IsNotAssignableFrom<T>(this IParamValidators self, string[] @params)
        {
            return self.IsNotAssignableFrom(typeof(T), @params);
        }

        internal static IParamValidators IsNotAssignableFrom(this IParamValidators self, Type expectedType, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name = p[i].Name;

                    return args =>
                    {
                        if (!args[i].GetType().IsAssignableFrom(expectedType))
                        { Throw("The type, '{0}', of the parameter '{1}' is assignable from {2} !", args[i].GetType(), name, expectedType); }
                    };
                });  
        }

        internal static IParamValidators IsEmpty(this IParamValidators self, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name =
                        p[i].Name;
                    
                    var getCount =
                        p[i].GetCount();

                    return args =>
                    {
                        if (((int)getCount(args[i], null)) > 0)
                        { Throw("The given value for '{0}' is not empty!", name, args[i]); }
                    };
                }); 
        }
        internal static IParamValidators IsNotEmpty(this IParamValidators self, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name =
                        p[i].Name;

                    var getCount =
                        p[i].GetCount();

                    return args =>
                    {
                        if (((int)getCount(args[i], null)) < 1)
                        { Throw("The given value for '{0}' is empty!", name, args[i]); }
                    };
                }); 
        }

        internal static IParamValidators IsFalse(this IParamValidators self, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name = p[i].Name;

                    return args =>
                    {
                        if ((bool)args[i])
                        { Throw("The given value for '{0}' is not false!", name); }
                    };
                });  
        }

        internal static IParamValidators IsTrue(this IParamValidators self, string[] @params)
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                {
                    var name = p[i].Name;

                    return args =>
                    {
                        if (!(bool)args[i])
                        { Throw("The given value for '{0}' is not true!", name); }
                    };
                });  
        }

        public static IParamValidators GreaterOrEqual<P>(this IParamValidators self, P higher, params string[] @params)
            where P: struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                args =>
                {
                    var name = p[i].Name;

                    if (!_Validate.GreaterOrEqual<P>(higher, (P)args[i]))
                    { Throw("The given value for '{0}' is not bigger than {1}!", name, higher); }
                });
        }

        public static IParamValidators LessOrEqual<P>(this IParamValidators self, P less, params string[] @params)
            where P: struct
        {
            return AddValidator(
                self,
                @params,
                (i, p) =>
                args =>
                {
                    var name = p[i].Name;

                    if (!_Validate.LessOrEqual<P>(less, (P)args[i]))
                    { Throw("The given value for '{0}' is not bigger than {1}!", name, less); }
                });
        }
    }
}