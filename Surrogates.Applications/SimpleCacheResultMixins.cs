using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Expressions;
using Surrogates.Utilities;
using System.Threading;
using Surrogates.Applications.Cache;
using System.Reflection;
using Surrogates.Tactics;
using Surrogates.Applications.Mixins;

namespace Surrogates.Applications
{
    public class CacheParams
    {
        public Func<object[], object> GetKey { get; set; }

        public TimeSpan Timeout { get; set; }
    }

    public static class SimpleCacheResultMixins
    {        
        private static Func<object[], object> GenerateKey(MethodInfo method)
        {
            var argTypes =
                method.GetParameters().Select(p => p.ParameterType).ToArray();

            //when it has more than one parameters, it creates a tuple for up to 8 
            if (argTypes.Length > 1)
            {                
                var tupleType = Type
                    .GetType(string.Concat("System.Tuple<", argTypes.Skip(1).Select(t => ',').ToArray(), ">"))
                    .MakeGenericType(argTypes);

                return args =>
                    Activator.CreateInstance(tupleType, args);
            }

            return argTypes.Length == 1 ?
                // if has one argument, brings it as key
                (Func<object[], object>)(args => args[0]) :
                // if not, a the simplest key
                args => true;
        }

        private static AndExpression<T> Cache<T>(this UsingInterferenceExpression<T> expr, Func<object[], object> getKey, TimeSpan? timeout)
        {            
            var strat =
                Pass.Current<Strategy.ForMethods>(expr);

            Func<MethodInfo, CacheParams> valueGetter = null;
            
            if(getKey != null)
            {
                valueGetter = m => 
                    new CacheParams 
                    { 
                        GetKey = getKey, 
                        Timeout = timeout.HasValue ? timeout.Value : new TimeSpan(0, 5, 0)
                    };
            }
            else
            {
                valueGetter = m => 
                    new CacheParams 
                    { 
                        GetKey = GenerateKey(m), 
                        Timeout = timeout.HasValue ? timeout.Value : new TimeSpan(0, 5, 0)
                    };
            }
            
            var newKeys = 
                strat.BaseMethods.ToDictionary(m => m.MethodHandle.Value, valueGetter);

            var @params = strat
                .NewProperties
                .FirstOrDefault(p => p.Name == "Params").DefaultValue as Dictionary<IntPtr, CacheParams>;

            if (@params == null)
            { @params = new Dictionary<IntPtr,CacheParams>(); }

            return expr
                 .Using<SimpleCacheInterceptor>("CacheMethod")
                 .And
                 .AddProperty<Dictionary<IntPtr, CacheParams>>("Params", @params != null ? @params.MergeLeft(newKeys) : newKeys);
        }   

        public static AndExpression<T> Cache<T>(this ApplyExpression<T> that, Func<T, Delegate> method, Func<object[], object> getKey = null, TimeSpan? timeout = null)
        {
            return that
                .PassOn()
                .Factory
                .Replace
                .This(method)
                .Cache( getKey, timeout);
        }

        public static AndExpression<T> Cache<T>(this ApplyExpression<T> that, Func<T, Delegate>[] methods, Func<object[], object> getKey = null, TimeSpan? timeout = null)
        {            
            return that
                .PassOn()
                .Factory
                .Replace
                .These(methods)
                .Cache(getKey, timeout);
        }

        public static AndExpression<T> Cache<T>(this ApplyExpression<T> that, string method, Func<object[], object> getKey = null, TimeSpan? timeout = null)
        {
            return that
                .PassOn()
                .Factory
                .Replace
                .Method(method)
                .Cache(getKey, timeout);
        }

        public static AndExpression<T> Cache<T>(this ApplyExpression<T> that, string[] methods, Func<object[], object> getKey = null, TimeSpan? timeout = null)
        {
            return that
                .PassOn()
                .Factory
                .Replace
                .Methods(methods)
                .Cache(getKey, timeout);
        }
    }
}

