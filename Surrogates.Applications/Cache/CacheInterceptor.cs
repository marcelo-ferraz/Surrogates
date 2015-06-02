using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.Cache
{
    public class SimpleCacheInterceptor
    {
        private CachedList _cache;

        public SimpleCacheInterceptor()
        {
            _cache = new CachedList();
        }

        public object CacheMethod(Delegate s_method, object[] args, Delegate p_GetKey, TimeSpan p_Timespan)
        {
            object result = null;

            var key =
                p_GetKey.DynamicInvoke(args);

            if (_cache.TryGet(key, ref result))
            { return result; }

            return _cache.Add(
                key,
                s_method.DynamicInvoke(args),
                p_Timespan);
        }
    }
}
