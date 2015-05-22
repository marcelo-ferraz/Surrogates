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
    public static class These
    {
        public class Validators
        {
            public IParamValidators Null(params string[] @params)
            {
                return ParameterAssertionMixins.Null(null, @params);
            }

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

            public IParamValidators Composite<T>(string param, params IPropValidators[] validators)
            {
                return ParameterAssertionMixins.Composite<T>(null, param, validators);
            }

            public IParamValidators Composite<T>(string[] @params, params IPropValidators[] validators)
            {
                return ParameterAssertionMixins.Composite<T>(null, @params, validators);
            }
            
            public IParamValidators Composite(params Delegate[] preValidators)
            {
                return ParameterAssertionMixins.Composite(null, preValidators);
            }
        }
         
        public static Validators Are { get; set; }

        static These()
        {
            Are = new These.Validators();             
        }
    }
}