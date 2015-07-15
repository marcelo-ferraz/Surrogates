using Surrogates.Applications.Cache;
using Surrogates.Applications.Cache.Model;
using Surrogates.Applications.Utilities;
using Surrogates.Expressions;
using Surrogates.Expressions.Accessors;
using Surrogates.Applications.Utilities;
using Surrogates.Tactics;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Surrogates.Applications.Model;

namespace Surrogates.Applications
{
    public static class SimpleCacheResultMixins
    {
        [TargetedPatchingOptOut("")]
        private static Func<object[], object> GenerateKey(MethodInfo method)
        {
            var argTypes = method
                .GetParameters()
                .Select(p => p.ParameterType)
                .ToArray()
                .Prepend(typeof(IntPtr));

            var handle =
                method.MethodHandle.Value;

            //when it has more than one parameters, it creates a tuple for up to 8 
            if (argTypes.Length > 1)
            {                
                var tupleType = Type
                    .GetType(string.Concat("System.Tuple<", argTypes.Skip(1).Select(t => ',').ToArray(), ">"))
                    .MakeGenericType(argTypes);

                return args =>
                    Activator.CreateInstance(tupleType, args.Prepend(handle));
            }

            return  args => handle;
        }

        [TargetedPatchingOptOut("")]
        private static AndExpression<T> Cache<T>(this AndExpression<T> expr, Func<object[], object> getKey, TimeSpan? timeout)
        {
            Func<MethodInfo, CacheParams> valueGetter = null;

            if (getKey != null)
            {
                valueGetter =
                    m =>
                        new CacheParams
                        {
                            GetKey = getKey,
                            Timeout = timeout.HasValue ? timeout.Value : new TimeSpan(0, 5, 0)
                        };
            }
            else
            {
                valueGetter =
                    m =>
                        new CacheParams
                        {
                            GetKey = GenerateKey(m),
                            Timeout = timeout.HasValue ? timeout.Value : new TimeSpan(0, 5, 0)
                        };
            }


            var @params = Pass
                .Current<Strategy>(expr)
                .MergeProperty("Params", valueGetter);

            return expr
                .And
                .AddProperty<Dictionary<IntPtr, CacheParams>>("Params", @params);
        }

        [TargetedPatchingOptOut("")]
        private static AndExpression<T> Cache<T>(this InterferenceExpression<T> expr, Func<object[], object> getKey, TimeSpan? timeout)
        {
            return expr
                .Accessors(a => a.Getter.Using<SimpleCacheInterceptor>("CacheMethod"))
                .Cache(getKey, timeout);        
        }

        [TargetedPatchingOptOut("")]
        private static AndExpression<T> Cache<T>(this UsingInterferenceExpression<T> expr, Func<object[], object> getKey, TimeSpan? timeout)
        {
            return expr
                .Using<SimpleCacheInterceptor>("CacheMethod")
                .Cache(getKey, timeout);            
        }

        /// <summary>
        /// Applies a Cache for a method's result, exempting that method's processing, for a given time
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The prior expression</param>
        /// <param name="prop"></param>
        /// <param name="key">How to get a key for that cache</param>
        /// <param name="timeout">How long it will live</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("")]
        public static AndExpression<T> Cache<T>(this ApplyExpression<T> that, Func<T, Delegate> method, Func<object[], object> key= null, TimeSpan? timeout = null)
        {
            return Pass
                .On<T, ShallowExtension<T>>(that)
                .Factory
                .Replace
                .This(method)
                .Cache( key, timeout);
        }
        
        /// <summary>
        /// Applies a Cache for a methods's result, exempting those methods's processing, for a given time 
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The prior expression</param>
        /// <param name="properties"></param>
        /// <param name="key">How to get a key for that cache</param>
        /// <param name="timeout">How long it will live</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("")]
        public static AndExpression<T> Cache<T>(this ApplyExpression<T> that, Func<T, Delegate>[] methods, Func<object[], object> key= null, TimeSpan? timeout = null)
        {
            return Pass
                .On<T, ShallowExtension<T>>(that)
                .Factory
                .Replace
                .These(methods)
                .Cache(key, timeout);
        }

        /// <summary>
        /// Applies a Cache for a method's result, exempting that method's processing, for a given time
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The prior expression</param>
        /// <param name="prop"></param>
        /// <param name="key">How to get a key for that cache</param>
        /// <param name="timeout">How long it will live</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("")]
        public static AndExpression<T> CacheMethod<T>(this ApplyExpression<T> that, string method, Func<object[], object> key= null, TimeSpan? timeout = null)
        {
            return Pass
                .On<T, ShallowExtension<T>>(that)
                .Factory
                .Replace
                .Method(method)
                .Cache(key, timeout);
        }

        /// <summary>
        /// Applies a Cache for a method's result, exempting those methods's processing, for a given time 
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The prior expression</param>
        /// <param name="properties"></param>
        /// <param name="key">How to get a key for that cache</param>
        /// <param name="timeout">How long it will live</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("")]
        public static AndExpression<T> CacheMethods<T>(this ApplyExpression<T> that, string[] methods, Func<object[], object> key= null, TimeSpan? timeout = null)
        {
            return Pass
                .On<T, ShallowExtension<T>>(that)
                .Factory
                .Replace
                .Methods(methods)
                .Cache(key, timeout);
        }

        /// <summary>
        /// Applies a Cache for a prop's result, exempting this property's processing, for a given time 
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The prior expression</param>
        /// <param name="prop"></param>
        /// <param name="key">How to get a key for that cache</param>
        /// <param name="timeout">How long it will live</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("")]
        public static AndExpression<T> Cache<T>(this ApplyExpression<T> that, Func<T, object> prop, Func<object[], object> key= null, TimeSpan? timeout = null)
        {
            return Pass
                .On<T, ShallowExtension<T>>(that)
                .Factory
                .Replace
                .This(prop)
                .Cache(key, timeout);
        }

        /// <summary>
        /// Applies a Cache for a prop's result, exempting those properties's processing, for a given time 
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The prior expression</param>
        /// <param name="properties"></param>
        /// <param name="key">How to get a key for that cache</param>
        /// <param name="timeout">How long it will live</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("")]
        public static AndExpression<T> Cache<T>(this ApplyExpression<T> that, Func<T, object>[] properties, Func<object[], object> key= null, TimeSpan? timeout = null)
        {
            return Pass
                .On<T, ShallowExtension<T>>(that)
                .Factory
                .Replace
                .These(properties)
                .Cache(key, timeout);
        }

        /// <summary>
        /// Applies a Cache for a prop's result, exempting this property's processing, for a given time 
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The prior expression</param>
        /// <param name="propertyName"></param>
        /// <param name="key">How to get a key for that cache</param>
        /// <param name="timeout">How long it will live</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("")]
        public static AndExpression<T> CacheProperty<T>(this ApplyExpression<T> that, string propertyName, Func<object[], object> key= null, TimeSpan? timeout = null)
        {
            return Pass
                .On<T, ShallowExtension<T>>(that)
                .Factory
                .Replace
                .Property(propertyName)
                .Cache(key, timeout);
        }

        /// <summary>
        /// Applies a Cache for a prop's result, exempting those properties's processing, for a given time 
        /// </summary>
        /// <typeparam name="T">The base type</typeparam>
        /// <param name="that">The prior expression</param>
        /// <param name="propertiesNames"></param>
        /// <param name="key">How to get a key for that cache</param>
        /// <param name="timeout">How long it will live</param>
        /// <returns></returns>
        [TargetedPatchingOptOut("")]
        public static AndExpression<T> CacheProperties<T>(this ApplyExpression<T> that, string[] propertiesNames, Func<object[], object> key= null, TimeSpan? timeout = null)
        {
            return Pass
                .On<T, ShallowExtension<T>>(that)
                .Factory
                .Replace
                .Properties(propertiesNames)
                .Cache(key, timeout);
        }
    }
}

