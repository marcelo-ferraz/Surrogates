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
            public IPreValidator Null(params string[] @params)
            {
                return ParameterAssertionMixins.Null(null, @params);
            }

            public IPreValidator Required(params string[] @params)
            {
                return ParameterAssertionMixins.Required(null, @params);
            }

            public IPreValidator Email(params string[] @params)
            {
                return ParameterAssertionMixins.Email(null, @params);
            }

            public IPreValidator Url(params string[] @params)
            {
                return ParameterAssertionMixins.Url(null, @params);
            }

            public IPreValidator Number(params string[] @params)
            {
                return ParameterAssertionMixins.Number(null, @params);
            }

            public IPreValidator InBetween<P>(P min, P max, params string[] @params)
                where P : struct
            {
                return ParameterAssertionMixins.InBetween(null, min, max, @params);
            }

            public IPreValidator BiggerThan<P>(P higher, params string[] @params)
                where P : struct
            {
                return ParameterAssertionMixins.BiggerThan(null, higher, @params);
            }

            public IPreValidator LowerThan<P>(P lower, params string[] @params)
                where P : struct
            {
                return ParameterAssertionMixins.LowerThan<P>(null, lower, @params);
            }

            public IPreValidator Regex(string expr, params string[] @params)
            {
                return ParameterAssertionMixins.Regex(null, expr, @params);
            }

            public IPreValidator Regex(Regex expr, params string[] @params)
            {
                return ParameterAssertionMixins.Regex(null, expr, @params);
            }

            public IPreValidator Composite<T>(string param, params IPropValidators[] validators)
            {
                return ParameterAssertionMixins.Composite<T>(null, param, validators);
            }

            public IPreValidator Composite<T>(string[] @params, params IPropValidators[] validators)
            {
                return ParameterAssertionMixins.Composite<T>(null, @params, validators);
            }
            
            public IPreValidator Composite(params Delegate[] preValidators)
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