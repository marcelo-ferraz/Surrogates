using System;

namespace Surrogates.Aspects.Cache.Model
{
    public class CacheParams
    {
        public Func<object[], object> GetKey { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}
