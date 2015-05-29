using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Surrogates.Utilities.Mixins;
using System.Runtime.Serialization;
using System.Reflection;
using System.Linq.Expressions;

namespace Surrogates.Applications.Contracts
{
    public static class Presume
    {        
        /// <summary>
        /// It presumes that the value of all the given parameters are equals to an expected value
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators AreEqual(object expected, params string[] parameters)
        {
            return ParameterAssertionMixins.AreEqual(null, expected, parameters);
        }

        /// <summary>
        /// It presumes that the reference of all the given parameters are equals to an expected reference
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators ReferenceEquals(object expected, params string[] parameters)
        {
            return ParameterAssertionMixins.AreReferenceEqual(null, expected, parameters);
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
        public static IParamValidators Contains(object expected, params string[] parameters)
        {
            return ParameterAssertionMixins.Contains(null, expected, parameters);
        }

        /// <summary>
        /// It presumes that an expected value is not contained inside a parameter's value. 
        /// <remarks>  It supports arrays, System.Collections.IList, System.Collections.IDictionary, System.Collections.Generic.ICollection<>, System.Collections.Generic.IDictionary<,>, System.Collections.Generic.IEnumerable<></remarks>
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators DoesNotContains(object expected, params string[] parameters)
        {
            return ParameterAssertionMixins.DoesNotContains(null, expected, parameters);
        }

        /// <summary>
        /// It presumes that the supplied parameters are assignable from a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsAssignableFrom<T>(params string[] parameters)
        {
            return ParameterAssertionMixins.IsAssignableFrom<T>(null, parameters);
        }

        /// <summary>
        /// It presumes that the supplied parameters are assignable from a given type
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsAssignableFrom(Type expected, params string[] parameters)
        {
            return ParameterAssertionMixins.IsAssignableFrom(null, expected, parameters);
        }

        /// <summary>
        /// It presumes that the supplied parameters are not assignable from a given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNotAssignableFrom<T>(params string[] parameters)
        {
            return ParameterAssertionMixins.IsNotAssignableFrom<T>(null, parameters);
        }

        /// <summary>
        /// It presumes that the supplied parameters are not assignable from a given type
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNotAssignableFrom(Type expected, params string[] parameters)
        {
            return ParameterAssertionMixins.IsNotAssignableFrom(null, expected, parameters);
        }
        
        /// <summary>
        /// It presumes that the value of the supplied parameters are empty
        /// <remarks>  It supports arrays, System.Collections.IList, System.Collections.IDictionary, System.Collections.Generic.ICollection<>, System.Collections.Generic.IDictionary<,>, System.Collections.Generic.IEnumerable<></remarks>
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsEmpty(params string[] parameters)//list or string
        {
            return ParameterAssertionMixins.IsEmpty(null, parameters);
        }

        /// <summary>
        /// It presumes that the value of the supplied parameters are not empty
        /// <remarks>  It supports arrays, System.Collections.IList, System.Collections.IDictionary, System.Collections.Generic.ICollection<>, System.Collections.Generic.IDictionary<,>, System.Collections.Generic.IEnumerable<></remarks>
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNotEmpty(params string[] parameters)//list or string
        {
            return ParameterAssertionMixins.IsNotEmpty(null, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter are false
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsFalse(params string[] parameters)
        {
            return ParameterAssertionMixins.IsFalse(null, parameters);
        }

        /// <summary>        
        /// It presumes that the value of the given parameter are true
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsTrue(params string[] parameters)
        {
            return ParameterAssertionMixins.IsTrue(null, parameters);
        }

        /// <summary>        
        /// It presumes that the value of the given parameter are null for reference types or the default value for value types
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNullOrDefault(params string[] parameters)
        {
            return ParameterAssertionMixins.IsNullOrDefault(null, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter are not null for reference types or not the default value for value types
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNotNullOrDefault(params string[] parameters)
        {
            return ParameterAssertionMixins.IsNotNullOrDefault(null, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is of an email. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsAnEmail(params string[] parameters)
        {
            return ParameterAssertionMixins.IsAnEmail(null, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is of an url. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsAnUrl(params string[] parameters)
        {
            return ParameterAssertionMixins.IsAnUrl(null, parameters);
        }
        
        /// <summary>
        /// It presumes that the value of the given parameter is of a number. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNumber(params string[] parameters)
        {
            return ParameterAssertionMixins.IsNumber(null, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is not of a number. Can only be used for string
        /// </summary>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsNaN(params string[] parameters)
        {
            return ParameterAssertionMixins.IsNaN(null, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is between a minimum and a maximum value
        /// </summary>
        /// <typeparam name="P">The type of those values</typeparam>
        /// <param name="min">The lower bound</param>
        /// <param name="max">The higher bound</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators IsInBetween<P>(P min, P max, params string[] parameters)
            where P : struct
        {
            return ParameterAssertionMixins.IsInBetween(null, min, max, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is greater than a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators Greater<P>(P expected, params string[] parameters)
            where P : struct
        {
            return ParameterAssertionMixins.Greater(null, expected, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is greater than or equals to a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators GreaterOrEqual<P>(P expected, params string[] parameters)
            where P : struct
        {
            return ParameterAssertionMixins.GreaterOrEqual<P>(null, expected, parameters);            
        }

        /// <summary>
        /// It presumes that the value of the given parameter is lesser than a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators Less<P>(P expected, params string[] parameters)
            where P : struct
        {
            return ParameterAssertionMixins.Less<P>(null, expected, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is lesser than or equals to a supplied value
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="expected"></param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators LessOrEqual<P>(P expected, params string[] parameters)
            where P : struct
        {
            return ParameterAssertionMixins.LessOrEqual<P>(null, expected, parameters);
        }
        
        /// <summary>
        /// It presumes that the value of the given parameter satisfies the given regular expression
        /// </summary>
        /// <param name="expr">A regular expression</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators ThisRegex(string expr, params string[] parameters)
        {
            return ParameterAssertionMixins.ThisRegex(null, expr, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter satisfies the given regular expression
        /// </summary>
        /// <param name="expr">A regular expression</param>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <returns></returns>
        public static IParamValidators ThisRegex(Regex expr, params string[] parameters)
        {
            return ParameterAssertionMixins.ThisRegex(null, expr, parameters);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is
        /// </summary>
        /// <param name="preValidators"></param>
        /// <returns></returns>
        public static IParamValidators That(params Delegate[] preValidators)
        {
            return ParameterAssertionMixins.That(null, preValidators);
        }

        #region to be implemented... or not?
        
        /// <summary>
        /// It presumes that the value of the given parameter is
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">The name of the parameter</param>
        /// <param name="validators">Validators</param>
        /// <returns></returns>
        private static IParamValidators That<T>(string param, params IPropValidators[] validators)
        {
            return ParameterAssertionMixins.That<T>(null, param, validators);
        }

        /// <summary>
        /// It presumes that the value of the given parameter is
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The actual name of the parameters</param>
        /// <param name="validators"></param>
        /// <returns></returns>
        private static IParamValidators That<T>(string[] parameters, params IPropValidators[] validators)
        {
            return ParameterAssertionMixins.That<T>(null, parameters, validators);
        }

        #endregion
    }
}