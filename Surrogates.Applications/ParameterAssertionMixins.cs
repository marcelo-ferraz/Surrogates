using Surrogates.Applications.Contracts;
using Surrogates.Applications.Contracts.Collections;
using Surrogates.Applications.Contracts.Model;
using Surrogates.Applications.Contracts.Utilities;
using Surrogates.Applications.Utilities;
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
        private static void ThrowNotValid(ParameterInfo[] p)
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

        private static IParamValidator AddPreValidators(IParamValidator self, string[] parameters, Func<int, ParameterInfo[], string, Action<object[]>> validator)
        {
            return AddPreValidators(
                self,
                parameters,
                (i, p) =>
                    validator(i, p, p[i].Name));                
        }

        private static IParamValidator AddPreValidators(IParamValidator assertions, string[] parameters, Func<int, ParameterInfo[], Action<object[]>> validator)
        {
            if (parameters.Length < 1)
            { throw new ArgumentException("You have to provide at least one parameter to be validated!"); }

            var ass = (AssertionList4Parameters)
                (assertions ?? (assertions = new AssertionList4Parameters()));
            
            for (int i = 0; i < parameters.Length; i++)
            {
                ass.Validators
                    .Add(new AssertionEntry4Parameters
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
        public static IParamValidator AreEqual(this IParamValidator self, object expected, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if (!object.Equals(expected, args[i]))
                        { Throw("The given value for '{0}', {1} is not equals to {2}!", name, args[i], expected); }
                    });
        }

        /// <summary>
        /// It presumes that the reference of all the given parameters are equals to an expected reference
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator AreReferenceEqual(this IParamValidator self, object expected, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) => 
                    args =>
                    {
                        if (!object.ReferenceEquals(expected, args[i]))
                        { Throw("The given value for '{0}', {1} is not equals to {2}!", name, args[i], expected); }
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
        public static IParamValidator Contains(this IParamValidator self, object expected, string[] on)
        {
            return AddPreValidators(
               self,
               on,
               (i, p, name) =>
               {
                   var contains =
                       Check.Contains(i, p, expected);               

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
        public static IParamValidator DoesNotContains(this IParamValidator self, object expected, string[] on)
        {
            return AddPreValidators(
               self,
               on,
               (i, p, name) =>
               {
                   var contains =
                       Check.Contains(i, p, expected);
                   
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
        public static IParamValidator IsAssignableFrom<T>(this IParamValidator self, string[] on)
        {
            return self.IsAssignableFrom(typeof(T), on);
        }

        /// <summary>
        /// It presumes that the supplied parameters are assignable from a given type
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsAssignableFrom(this IParamValidator self, Type expectedType, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) => args =>
                    {
                        if (args[i].GetType().IsAssignableFrom(expectedType))
                        { Throw("The type, '{0}', of the parameter '{1}' is not assignable from {2} !", args[i].GetType(), name, expectedType); }
                    });
        }

        /// <summary>
        /// It presumes that the supplied parameters are not assignable from a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsNotAssignableFrom<T>(this IParamValidator self, string[] on)
        {
            return self.IsNotAssignableFrom(typeof(T), on);
        }

        /// <summary>
        /// It presumes that the supplied parameters are not assignable from a given type
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsNotAssignableFrom(this IParamValidator self, Type expectedType, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if (!args[i].GetType().IsAssignableFrom(expectedType))
                        { Throw("The type, '{0}', of the parameter '{1}' is assignable from {2} !", args[i].GetType(), name, expectedType); }
                    });
        }

        /// <summary>
        /// It presumes that the value of the supplied parameters are empty
        /// <remarks>  It supports arrays, System.Collections.IList, System.Collections.IDictionary, System.Collections.Generic.ICollection<>, System.Collections.Generic.IDictionary<,>, System.Collections.Generic.IEnumerable<></remarks>
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsEmpty(this IParamValidator self, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                {
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
        public static IParamValidator IsNotEmpty(this IParamValidator self, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                {
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
        public static IParamValidator IsFalse(this IParamValidator self, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if ((bool)args[i])
                        { Throw("The given value for '{0}' is not false!", name); }
                    });
        }

        /// <summary>        
        /// It presumes that the value of the given parameter are true
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsTrue(this IParamValidator self, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if (!(bool)args[i])
                        { Throw("The given value for '{0}' is not true!", name); }
                    });
        }

        /// <summary>        
        /// It presumes that the value of the given parameter are null for reference types or the default value for value types
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsNullOrDefault(this IParamValidator self, params string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                {                    
                    var type = p[i].ParameterType;

                    if (p[i].ParameterType == typeof(string))
                    {
                        return args =>
                        {
                            if (!string.IsNullOrEmpty((string)args[i]))
                            { Throw("The given value for '{0}' is not the default of {1}!", name, type); }
                        };
                    }

                    return args =>
                    {
                        if (!Check.IsNotNullOrDefault(args[i], type))
                        { Throw("The given value for '{0}' is not the default of {1}!", name, type); }
                    };
                });
        }

        /// <summary>
        /// It presumes that the value of the given parameter are not null for reference types or not the default value for value types
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsNotNullOrDefault(this IParamValidator self, params string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                {
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
                        if (!Check.IsNotNullOrDefault(args[i], type))
                        { Throw("The given value for '{0}' is required!", name); }
                    };
                });
        }

        /// <summary>
        /// It presumes that the value of the given parameter is of an email. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsAnEmail(this IParamValidator self, params string[] on)
        {
            return ThisRegex(self, Check.EmailRegexpr, on);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is of an url. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsAnUrl(this IParamValidator self, params string[] on)
        {
            return ThisRegex(self, Check.UrlRegexpr, on);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is of a number. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator IsNumber(this IParamValidator self, params string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                {                    
                    var expr = Check.IsNumberRegexpr;
                    return args =>
                    {
                        if (!Check.Regex(expr, (string)args[i]))
                        { Throw("The given value for '{0}' is not a number!", name); }
                    };
                });
        }

        /// <summary>
        /// It presumes that the value of the given parameter is not of a number. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        internal static IParamValidator IsNaN(this IParamValidator self, string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                {                    
                    var expr = Check.IsNumberRegexpr;
                    return args =>
                    {
                        if (Check.Regex(expr, (string)args[i]))
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
        public static IParamValidator IsInBetween<P>(this IParamValidator self, P min, P max, params string[] on)
            where P : struct
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                        args =>
                        {
                            if (!Check.InBetween<P>(min, max, (P)args[i]))
                            { Throw("The given value for '{0}' is not in between of {1} and {2}!", name, min, max); }
                        });
        }

        /// <summary>
        /// It presumes that the value of the given parameter is greater than a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator Greater<P>(this IParamValidator self, P higher, params string[] on)
            where P : struct
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if (!Check.Greater<P>(higher, (P)args[i]))
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
        public static IParamValidator GreaterOrEqual<P>(this IParamValidator self, P higher, params string[] on)
            where P : struct
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if (!Check.GreaterOrEqual<P>(higher, (P)args[i]))
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
        public static IParamValidator Less<P>(this IParamValidator self, P lower, params string[] on)
            where P : struct
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if (!Check.Less<P>(lower, (P)args[i]))
                        { Throw("The given value for '{0}' is not lower than {1}!", name, lower); }
                    });
        }

        /// <summary>
        /// It presumes that the value of the given parameter is lesser than or equals to a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator LessOrEqual<P>(this IParamValidator self, P less, params string[] on)
            where P : struct
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if (!Check.LessOrEqual<P>(less, (P)args[i]))
                        { Throw("The given value for '{0}' is not bigger than {1}!", name, less); }
                    });
        }

        /// <summary>
        /// It presumes that the value of the given parameter satisfies the given regular expression
        /// </summary>
        /// <param name="expr">A regular expression</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator ThisRegex(this IParamValidator self, string expr, params string[] on)
        {
            return ThisRegex(self, new Regex(expr), on);
        }

        /// <summary>
        /// It presumes that the value of the given parameter satisfies the given regular expression
        /// </summary>
        /// <param name="expr">A regular expression</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidator ThisRegex(this IParamValidator self, Regex expr, params string[] on)
        {
            return AddPreValidators(
                self,
                on,
                (i, p, name) =>
                    args =>
                    {
                        if (!Check.Regex(expr, (string)args[i]))
                        { Throw("The given value for '{0}' does not match the expression '{1}'!", name, expr.ToString()); }
                    });
        }

        /// <summary>
        /// It presumes that the value of the given parameter is
        /// </summary>
        /// <param name="preValidators"></param>
        /// <returns></returns>
        public static IParamValidator That(this IParamValidator self, params Delegate[] preValidators)
        {
            var ass = (AssertionList4Parameters)
                (self ?? (self = new AssertionList4Parameters()));

            foreach (var preValidator in preValidators)
            {
                var on =
                    preValidator.Method.GetParameters();

                if (on.Length < 1) { continue; }

                AddPreValidators(
                    self,
                    new[] { on[0].Name },
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
                                ThrowNotValid(p);
                            }
                        };
                    });
            }

            return self;
        }


        #region To be implemented, ... or not ?

        //internal static IParamValidator That<T>(this IParamValidator self, string param, params IPropValidator[] validators)
        //{
        //    return That<T>(self, new string[] { param }, validators);
        //}

        //internal static IParamValidator That<T>(this IParamValidator self, string[] on, params IPropValidator[] validators)
        //{
        //    Func<T, bool> assertion = null;

        //    foreach (var validator in validators.SelectMany(v => ((AssertionList4Properties)v).Validators))
        //    {
        //        assertion = assertion != null ?
        //            (arg => assertion(arg) && (bool)validator.Validation.DynamicInvoke(arg)) :
        //            (Func<T, bool>)validator.Validation;
        //    }

        //    return AddPreValidators(
        //        self,
        //        on,
        //        (i, p) =>
        //            args => assertion((T)args[i]));
        //}

        #endregion
    }
}