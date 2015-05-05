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