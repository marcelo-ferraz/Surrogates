using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.Pooling
{
    public class PoolInterceptor<T>
        where T : IDisposable
    {
        private Pool<T> _pool;
        private SurrogatesContainer _container;

        public PoolInterceptor()
        {
            _container = new SurrogatesContainer();

            _container.Map(m =>
                m.From<T>()
                .Visit
                .This(x => (Action)x.Dispose)
                .Using<PoolInterceptor<T>>(i => (Action<Pool<T>, T>) i.Dispose));

            _pool = new Pool<T>(5, p => _container.Invoke<T>());
        }                
        
        internal T Get()
        {
            return _pool.Acquire();
        }

        internal void Dispose(Pool<T> s_pool, T s_instance)
        {
            _pool.Release(s_instance);            
        }
    }
}
