using System;

namespace Surrogates.Applications.Cache.Model
{
    public class CacheParams
    {
        public Func<object[], object> GetKey { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}
