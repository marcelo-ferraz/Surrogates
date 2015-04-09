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

            _container.Map(m => m
                .From<T>()
                .Visit
                .This(x => (Action)x.Dispose)
                .Using<PoolInterceptor<T>>(i => (Action<object>) i.Dispose));

            _pool = new Pool<T>(5, p => _container.Invoke<T>(stateBag: this));
        }                
        
        internal T Get()
        {
            return _pool.Acquire();
        }

        internal void Dispose(dynamic s_StateBag)
        {
            s_StateBag._pool.Release(default(T));            
        }
    }
}
