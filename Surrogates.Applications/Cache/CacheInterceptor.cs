using Surrogates.Aspects.Cache.Model;
using System;
using System.Collections.Generic;

namespace Surrogates.Aspects.Cache
{
    public class SimpleCacheInterceptor
    {
        private static CachedList _cache;

        static SimpleCacheInterceptor()
        {
            _cache = new CachedList();
        }

        public object CacheMethod(Delegate s_method, object[] args, Dictionary<IntPtr, CacheParams> p_Params)
        {
            object result = null;

            var thisPtr = s_method
                .Method
                .MethodHandle
                .Value;

            var @params =
                p_Params[thisPtr];

            var key = @params
                .GetKey
                .DynamicInvoke(new object[] { args });

            if(!_cache.TryGet(key, ref result))
            {
                result = s_method
                    .DynamicInvoke(args);

                _cache.Add(key, result, @params.Timeout);
            }

            return result;
        }
    }
}
