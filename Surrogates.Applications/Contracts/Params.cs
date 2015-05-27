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
    public static class Opine
    {        
        public static IParamValidators AreEqual(object expected, params string[] @params)
        {
            return ParameterAssertionMixins.AreEqual(null, expected, @params);
        }

        public static IParamValidators ReferenceEquals(object expected, params string[] @params)
        {
            return ParameterAssertionMixins.AreReferenceEqual(null, expected, @params);
        }


        public static IParamValidators Contains(object expected, params string[] @params)
        {
            return ParameterAssertionMixins.Contains(null, expected, @params);
        }

        public static IParamValidators DoesNotContains(object expected, params string[] @params)
        {
            return ParameterAssertionMixins.DoesNotContains(null, expected, @params);
        }

        public static IParamValidators IsAssignableFrom<T>(params string[] @params)
        {
            return ParameterAssertionMixins.IsAssignableFrom<T>(null, @params);
        }

        public static IParamValidators IsAssignableFrom(Type expected, params string[] @params)
        {
            return ParameterAssertionMixins.IsAssignableFrom(null, expected, @params);
        }

        public static IParamValidators IsNotAssignableFrom<T>(params string[] @params)
        {
            return ParameterAssertionMixins.IsNotAssignableFrom<T>(null, @params);
        }

        public static IParamValidators IsNotAssignableFrom(Type expected, params string[] @params)
        {
            return ParameterAssertionMixins.IsNotAssignableFrom(null, expected, @params);
        }

        public static IParamValidators IsEmpty(params string[] @params)//list or string
        {
            return ParameterAssertionMixins.IsEmpty(null, @params);
        }
        
        public static IParamValidators IsNotEmpty(params string[] @params)//list or string
        {
            return ParameterAssertionMixins.IsNotEmpty(null, @params);
        }

        public static IParamValidators IsFalse(params string[] @params)
        {
            return ParameterAssertionMixins.IsFalse(null, @params);
        }

        public static IParamValidators IsTrue(params string[] @params)
        {
            return ParameterAssertionMixins.IsTrue(null, @params);
        }

        public static IParamValidators IsNullOrDefault(params string[] @params)
        {
            return ParameterAssertionMixins.IsNullOrDefault(null, @params);
        }

        public static IParamValidators IsNotNullOrDefault(params string[] @params)
        {
            return ParameterAssertionMixins.IsNotNullOrDefault(null, @params);
        }

        public static IParamValidators IsAnEmail(params string[] @params)
        {
            return ParameterAssertionMixins.IsAnEmail(null, @params);
        }

        public static IParamValidators IsAnUrl(params string[] @params)
        {
            return ParameterAssertionMixins.IsAnUrl(null, @params);
        }

        public static IParamValidators IsNumber(params string[] @params)
        {
            return ParameterAssertionMixins.IsNumber(null, @params);
        }

        public static IParamValidators IsNaN(params string[] @params)
        {
            return ParameterAssertionMixins.IsNaN(null, @params);
        }

        public static IParamValidators IsInBetween<P>(P min, P max, params string[] @params)
            where P : struct
        {
            return ParameterAssertionMixins.IsInBetween(null, min, max, @params);
        }

        public static IParamValidators Greater<P>(P greater, params string[] @params)
            where P : struct
        {
            return ParameterAssertionMixins.Greater(null, greater, @params);
        }

        public static IParamValidators GreaterOrEqual<P>(P greater, params string[] @params)
            where P : struct
        {
            return ParameterAssertionMixins.GreaterOrEqual<P>(null, greater, @params);            
        }

        public static IParamValidators Less<P>(P lower, params string[] @params)
            where P : struct
        {
            return ParameterAssertionMixins.IsLowerThan<P>(null, lower, @params);
        }

        public static IParamValidators LessOrEqual<P>(P lower, params string[] @params)
            where P : struct
        {
            return ParameterAssertionMixins.LessOrEqual<P>(null, lower, @params);
        }
        
        public static IParamValidators ThisRegex(string expr, params string[] @params)
        {
            return ParameterAssertionMixins.ThisRegex(null, expr, @params);
        }

        public static IParamValidators ThisRegex(Regex expr, params string[] @params)
        {
            return ParameterAssertionMixins.ThisRegex(null, expr, @params);
        }

        public static IParamValidators That<T>(string param, params IPropValidators[] validators)
        {
            return ParameterAssertionMixins.That<T>(null, param, validators);
        }

        public static IParamValidators That<T>(string[] @params, params IPropValidators[] validators)
        {
            return ParameterAssertionMixins.That<T>(null, @params, validators);
        }
            
        public static IParamValidators That(params Delegate[] preValidators)
        {
            return ParameterAssertionMixins.That(null, preValidators);
        }        
    }
}