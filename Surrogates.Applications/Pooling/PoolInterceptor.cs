using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.Pooling
{
    public class PoolInterceptor<T>
    {
        protected Pool<T> _pool;
        
        internal T Get()
        {
            return _pool.Acquire();
        }

        internal void Dispose(T s_instance)
        {
            _pool.Release(s_instance);            
        }
    }
}
