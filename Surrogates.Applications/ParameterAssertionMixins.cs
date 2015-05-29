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
        private static IParamValidators AddValidator(IParamValidators assertions, string[] parameters, Func<int, ParameterInfo[], Action<object[]>> validator)
        {
            if (parameters.Length < 1)
            { throw new ArgumentException("You have to provide at least one parameter to be validated!"); }

            var ass = (Assert_.List4.Parameters)
                (assertions ?? (assertions = new Assert_.List4.Parameters()));
            

            for (int i = 0; i < parameters.Length; i++)
            {
                ass.Validators
                    .Add(new Assert_.Entry4.Parameters
                    {
                        ParameterName = parameters[i],
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

        /// <summary>
        /// It presumes that the value of all the given parameters are equals to an expected value
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators AreEqual(this IParamValidators self, object expected, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the reference of all the given parameters are equals to an expected reference
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators AreReferenceEqual(this IParamValidators self, object expected, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that an expected value is contained inside a parameter's value. 
        /// <remarks>
        ///  It supports arrays, System.Collections.IList, System.Collections.IDictionary, System.Collections.Generic.ICollection<>, System.Collections.Generic.IDictionary<,>, System.Collections.Generic.IEnumerable<>
        /// </remarks>
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators Contains(this IParamValidators self, object expected, string[] on)
        {
            return AddValidator(
               self,
               on,
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

        /// <summary>
        /// It presumes that an expected value is not contained inside a parameter's value. 
        /// <remarks>  It supports arrays, System.Collections.IList, System.Collections.IDictionary, System.Collections.Generic.ICollection<>, System.Collections.Generic.IDictionary<,>, System.Collections.Generic.IEnumerable<></remarks>
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators DoesNotContains(this IParamValidators self, object expected, string[] on)
        {
            return AddValidator(
               self,
               on,
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

        /// <summary>
        /// It presumes that the supplied parameters are assignable from a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsAssignableFrom<T>(this IParamValidators self, string[] on)
        {
            return self.IsAssignableFrom(typeof(T), on);
        }

        /// <summary>
        /// It presumes that the supplied parameters are assignable from a given type
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsAssignableFrom(this IParamValidators self, Type expectedType, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the supplied parameters are not assignable from a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNotAssignableFrom<T>(this IParamValidators self, string[] on)
        {
            return self.IsNotAssignableFrom(typeof(T), on);
        }

        /// <summary>
        /// It presumes that the supplied parameters are not assignable from a given type
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNotAssignableFrom(this IParamValidators self, Type expectedType, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the value of the supplied parameters are empty
        /// <remarks>  It supports arrays, System.Collections.IList, System.Collections.IDictionary, System.Collections.Generic.ICollection<>, System.Collections.Generic.IDictionary<,>, System.Collections.Generic.IEnumerable<></remarks>
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsEmpty(this IParamValidators self, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the value of the supplied parameters are not empty
        /// <remarks>  It supports arrays, System.Collections.IList, System.Collections.IDictionary, System.Collections.Generic.ICollection<>, System.Collections.Generic.IDictionary<,>, System.Collections.Generic.IEnumerable<></remarks>
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNotEmpty(this IParamValidators self, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the value of the given parameter are false
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsFalse(this IParamValidators self, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>        
        /// It presumes that the value of the given parameter are true
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsTrue(this IParamValidators self, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>        
        /// It presumes that the value of the given parameter are null for reference types or the default value for value types
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNullOrDefault(this IParamValidators self, params string[] on)
        {
            return AddValidator(
                self,
                on,
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
        
        /// <summary>
        /// It presumes that the value of the given parameter are not null for reference types or not the default value for value types
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNotNullOrDefault(this IParamValidators self, params string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the value of the given parameter is of an email. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsAnEmail(this IParamValidators self, params string[] on)
        {
            return ThisRegex(self, _Validate.EmailRegexpr, on);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is of an url. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsAnUrl(this IParamValidators self, params string[] on)
        {
            return ThisRegex(self, _Validate.UrlRegexpr, on);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is of a number. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNumber(this IParamValidators self, params string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the value of the given parameter is not of a number. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        internal static IParamValidators IsNaN(this IParamValidators self, string[] on)
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the value of the given parameter is between a minimum and a maximum value
        /// </summary>
        /// <typeparam name="P">The type of those values</typeparam>
        /// <param name="min">The lower bound</param>
        /// <param name="max">The higher bound</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsInBetween<P>(this IParamValidators self, P min, P max, params string[] on)
            where P : struct
        {
            return AddValidator(
                self,
                on,
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

        /// <summary>
        /// It presumes that the value of the given parameter is greater than a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators Greater<P>(this IParamValidators self, P higher, params string[] on)
            where P : struct
        {
            return AddValidator(
                self,
                on,
                (i, p) =>
                    args => {
                        var name = p[i].Name;
                        
                        if(!_Validate.Greater<P>(higher, (P)args[i]))
                        { Throw("The given value for '{0}' is not bigger than {1}!", name, higher); }                        
                    });
        }

        /// <summary>
        /// It presumes that the value of the given parameter is greater than or equals to a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators GreaterOrEqual<P>(this IParamValidators self, P higher, params string[] on)
            where P : struct
        {
            return AddValidator(
                self,
                on,
                (i, p) =>
                args =>
                {
                    var name = p[i].Name;

                    if (!_Validate.GreaterOrEqual<P>(higher, (P)args[i]))
                    { Throw("The given value for '{0}' is not bigger than {1}!", name, higher); }
                });
        }


        /// <summary>
        /// It presumes that the value of the given parameter is lesser than a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators Less<P>(this IParamValidators self, P lower, params string[] on)
            where P : struct
        {
            return AddValidator(
                self,
                on,
                (i, p) => {
                    var name = p[i].Name;

                    return args =>
                    {
                        if (!_Validate.Less<P>(lower, (P)args[i]))
                        { Throw("The given value for '{0}' is not lower than {1}!", name, lower); }
                    };
                });                    
        }

        /// <summary>
        /// It presumes that the value of the given parameter is lesser than or equals to a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators LessOrEqual<P>(this IParamValidators self, P less, params string[] on)
            where P : struct
        {
            return AddValidator(
                self,
                on,
                (i, p) =>
                args =>
                {
                    var name = p[i].Name;

                    if (!_Validate.LessOrEqual<P>(less, (P)args[i]))
                    { Throw("The given value for '{0}' is not bigger than {1}!", name, less); }
                });
        }

        /// <summary>
        /// It presumes that the value of the given parameter satisfies the given regular expression
        /// </summary>
        /// <param name="expr">A regular expression</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators ThisRegex(this IParamValidators self, string expr, params string[] on)
        {
            return ThisRegex(self, new Regex(expr), on);
        }

        /// <summary>
        /// It presumes that the value of the given parameter satisfies the given regular expression
        /// </summary>
        /// <param name="expr">A regular expression</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators ThisRegex(this IParamValidators self, Regex expr, params string[] on)
        {
            return AddValidator(
                self,
                on,
                (i, p) =>{
                    var name = p[i].Name;
                    
                    return args => {
                        if(!_Validate.Regex(expr, (string)args[i]))
                        { Throw("The given value for '{0}' does not match the expression '{1}'!", name, expr.ToString()); }                        
                    };
                });                   
        }

        /// <summary>
        /// It presumes that the value of the given parameter is
        /// </summary>
        /// <param name="preValidators"></param>
        /// <returns></returns>
        public static IParamValidators That(this IParamValidators self, params Delegate[] preValidators)
        {
            var ass = (Assert_.List4.Parameters)
                (self ?? (self = new Assert_.List4.Parameters()));
            
            foreach (var preValidator in preValidators)
            {
                var on = 
                    preValidator.Method.GetParameters();

                if(on.Length < 1) { continue; }

                AddValidator(
                    self,
                    new [] { on[0].Name },
                    (j, p) => 
                    {
                        var indexes = new List<int>();
                        foreach (var valParam in on)
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

        #region To be implemented, ... or not ?

        internal static IParamValidators That<T>(this IParamValidators self, string param, params IPropValidators[] validators)
        {
            return That<T>(self, new string[] { param }, validators);
        }

        internal static IParamValidators That<T>(this IParamValidators self, string[] on, params IPropValidators[] validators)
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
                on,
                (i, p) =>
                    args => assertion((T)args[i]));
        }

        #endregion
    }
}