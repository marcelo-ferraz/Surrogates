using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Surrogates.Utilities.Mixins;
using System.Runtime.Serialization;
using System.Reflection;

namespace Surrogates.Applications.Validators
{
    public class ParametersValidator<T> : BaseValidator
    {
        private static T _uninitObj = (T)
            FormatterServices.GetUninitializedObject(typeof(T));

        private Func<T, Delegate>[] ToMethods(params string[] names)
        {
            var methods = (Func<T, Delegate>[])
                Array.CreateInstance(typeof(Func<T, Delegate>), names.Length);

            for (int i = 0; i < names.Length; i++)
            {
                methods[i] = (Func<T, Delegate>) Delegate.CreateDelegate(
                    typeof(T), typeof(T).GetMethod4Surrogacy(names[i]));
            }

            return methods;
        }

        private static IDictionary<string, Func<object[], bool>> AddValidator(Func<T, Delegate>[] methods, string[] parameterNames, Func<int, ParameterInfo, Func<object[], bool>> validator)
        {
            var result = 
                new Dictionary<string, Func<object[], bool>>();

            foreach (var getMethod in methods)
            {
                var method = getMethod(_uninitObj).Method;

                var @params = 
                    method.GetParameters();

                Func<object[], bool> methodValidator = null;

                foreach(var name in parameterNames)
                {
                    Func<object[], bool> paramValidate;

                    for (var i = 0; i < @params.Length; i++)
                    {
                        if (@params[i].Name != name) { continue; }

                        paramValidate = validator(i, @params[i]);

                        if (paramValidate != null)
                        { continue; }
                                                
                        methodValidator = (Func<object[], bool>)(result != null ?
                            Delegate.Combine(methodValidator, paramValidate) : 
                            paramValidate);
                    }
                }

                if (methodValidator != null)
                { result.Add(method.Name, methodValidator); }
            }

            return result;
        }

        public IDictionary<string, Func<object[], bool>> Required(string method, params string[] parameters)
        {
            return Required(new [] { method }, parameters);
        }

        public IDictionary<string, Func<object[], bool>> Required(string[] methods, params string[] parameters)
        {
            return Required(ToMethods(methods), parameters);
        }

        public IDictionary<string, Func<object[], bool>> Required(Func<T, Delegate>[] methods, params string[] parameterNames)
        {
            return AddValidator(
                methods,
                parameterNames,
                (i, p) =>
                {
                    if (p.ParameterType == typeof(string))
                    {
                        return args =>
                        {
                            var val = args[i];
                            var t = val.GetType();

                            return (t == typeof(string) && string.IsNullOrEmpty((string)val)) ?
                                false :
                                val == Activator.CreateInstance(t);
                        };
                    }

                    return args =>
                    {
                        var val = args[i];
                        var t = val.GetType();

                        return (!t.IsValueType || Nullable.GetUnderlyingType(t) != null) ?
                            val == null :
                            val == Activator.CreateInstance(t);
                    };
                });
        }

        public IDictionary<string, Func<object[], bool>> Email(string method, params string[] parameters)
        {
            return Email(new []{ method }, parameters);
        }

        public IDictionary<string, Func<object[], bool>> Email(string[] methods, params string[] parameters)
        {
            return Email(ToMethods(methods), parameters);
        }

        public IDictionary<string, Func<object[], bool>> Email(Func<T, Delegate> method, params string[] parameters)
        {
            return Email(new[] { method }, parameters);
        }

        public IDictionary<string, Func<object[], bool>> Email(Func<T, Delegate>[] methods, params string[] parameters)
        {
            return Regex(EmailRegexpr, methods, parameters);
        }

        public IDictionary<string, Func<object[], bool>> Url(string method, params string[] parameters)
        {
            return Url(new[] { method }, parameters);
        }

        public IDictionary<string, Func<object[], bool>> Url(string[] methods, params string[] parameters)
        {
            return Url(ToMethods(methods), parameters);
        }

        public IDictionary<string, Func<object[], bool>> Url(Func<T, Delegate> method, params string[] parameters)
        {
            return Url(new[] { method }, parameters);
        }

        public IDictionary<string, Func<object[], bool>> Url(Func<T, Delegate>[] methods, params string[] parameters)
        {
            return Regex(UrlRegexpr, methods, parameters);
        }

        public IDictionary<string, Func<object[], bool>> IsNumber(string method, params string[] parameters)
        {
            return IsNumber(new[] { method }, parameters);
        }

        public IDictionary<string, Func<object[], bool>> IsNumber(string[] methods, params string[] parameters)
        {
            return IsNumber(ToMethods(methods), parameters);
        }

        public IDictionary<string, Func<object[], bool>> IsNumber(Func<T, Delegate> method, params string[] parameters)
        {
            return IsNumber(new[] { method }, parameters);
        }

        public IDictionary<string, Func<object[], bool>> IsNumber(Func<T, Delegate>[] methods, params string[] parameters)
        {
            return Regex(IsNumberRegexpr, methods, parameters);
        }

        public IDictionary<string, Func<object[], bool>> InBetween<P>(P min, P max, string[] method, params string[] parameterNames)
            where P : struct
        {
            return InBetween<P>(min, max, ToMethods(method), parameterNames);
        }

        public IDictionary<string, Func<object[], bool>> InBetween<P>(P min, P max, string method, params string[] parameterNames)
            where P : struct
        {
            return InBetween<P>(min, max, ToMethods(method), parameterNames);
        }

        public IDictionary<string, Func<object[], bool>> InBetween<P>(P min, P max, Func<T, Delegate> method, params string[] parameterNames)
            where P : struct
        {
            return InBetween<P>(min, max, new [] { method }, parameterNames);
        }

        public IDictionary<string, Func<object[], bool>> InBetween<P>(P min, P max, Func<T, Delegate>[] methods, params string[] parameterNames)
            where P : struct
        {         
            return AddValidator(
                methods,
                parameterNames,
                (i, p) =>
                    args =>
                        (Comparer<P>.Default.Compare(min, (P) args[i]) > 0) &&
                        (Comparer<P>.Default.Compare(max, (P) args[i]) < 0));
        }

        public IDictionary<string, Func<object[], bool>>  BiggerThan<P>(P lower, string[] method, params string[] parameterNames)
            where P : struct
        {
            return BiggerThan(lower, ToMethods(method), parameterNames);
        }
        
        public IDictionary<string, Func<object[], bool>>  BiggerThan<P>(P lower, string method, params string[] parameterNames)
            where P : struct
        {
            return BiggerThan(lower, ToMethods(method), parameterNames);
        }

        public IDictionary<string, Func<object[], bool>> BiggerThan<P>(P lower, Func<T, Delegate> method, params string[] parameterNames)
            where P : struct
        {
            return BiggerThan(lower, new[] { method }, parameterNames);
        }

        public IDictionary<string, Func<object[], bool>> BiggerThan<P>(P lower, Func<T, Delegate>[] methods, params string[] parameterNames)
            where P : struct
        {
            return AddValidator(
                methods,
                parameterNames,
                (i, p) =>
                    args =>
                        (Comparer<P>.Default.Compare(lower, (P) args[i]) > 0));
        }
        
        public IDictionary<string, Func<object[], bool>> LowerThan<P>(P lower, string[] method, params string[] parameterNames)
            where P : struct
        {
            return LowerThan(lower, ToMethods(method), parameterNames);
        }

        public IDictionary<string, Func<object[], bool>> LowerThan<P>(P lower, string method, params string[] parameterNames)
            where P : struct
        {
            return LowerThan(lower, ToMethods(method), parameterNames);
        }

        public IDictionary<string, Func<object[], bool>> LowerThan<P>(P lower, Func<T, Delegate> method, params string[] parameterNames)
            where P : struct
        {
            return LowerThan(lower, new[] { method }, parameterNames);
        }

        public IDictionary<string, Func<object[], bool>> LowerThan<P>(P lower, Func<T, Delegate>[] methods, params string[] parameterNames)
            where P : struct
        {
            return AddValidator(
                methods,
                parameterNames,
                (i, p) =>
                    args =>
                        (Comparer<P>.Default.Compare(lower, (P) args[i]) < 0));
        }

        public IDictionary<string, Func<object[], bool>> Regex(string expr, string name, params string[] @params)
        {
            return Regex(new Regex(expr), ToMethods(name), @params);
        }
        public IDictionary<string, Func<object[], bool>> Regex(string expr, string[] names, params string[] @params)
        {
            return Regex(new Regex(expr), ToMethods(names), @params);
        }

        public IDictionary<string, Func<object[], bool>> Regex(Regex expr, string name, params string[] @params)
        {
            return Regex(expr, ToMethods(name), @params);
        }
        public IDictionary<string, Func<object[], bool>> Regex(Regex expr, string[] names, params string[] @params)
        {
            return Regex(expr, ToMethods(names), @params);
        }

        public IDictionary<string, Func<object[], bool>> Regex(string expr, Func<T, Delegate> method, params string[] @params)
        {
            return Regex(new Regex(expr), new[] { method }, @params);
        }
        public IDictionary<string, Func<object[], bool>> Regex(string expr, Func<T, Delegate>[] methods, params string[] @params)
        {
            return Regex(new Regex(expr), methods, @params);
        }

        public IDictionary<string, Func<object[], bool>> Regex(Regex expr, Func<T, Delegate> method, params string[] @params)
        {
            return Regex(expr, new [] { method }, @params);
        }

        public IDictionary<string, Func<object[], bool>> Regex(Regex expr, Func<T, Delegate>[] methods, params string[] @params)
        {
            return AddValidator(
                methods,
                @params,
                (i, p) => {
                    return args => 
                        expr.IsMatch((string) args[i]);
                });
        }
    }
}
