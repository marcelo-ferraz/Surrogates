using System;

namespace Surrogates.Applications.Pooling
{
    public class PoolInterceptor<T>
        where T : IDisposable
    {
        private Pool<T> _pool;
        
        private bool _initiated;

        public PoolInterceptor()
        {
        }

        internal T Get(dynamic s_StateBag, SurrogatesContainer s_Container)
        {
            if (!_initiated)
            {
                _pool = new Pool<T>(
                    (int) s_StateBag.Size, 
                    p => s_Container.Invoke<T>(stateBag: this),
                    (LoadingMode) s_StateBag.LoadingMode,
                    (AccessMode) s_StateBag.AccessMode);
            }

            return _pool.Acquire();
        }

        internal void Dispose(dynamic s_StateBag, T s_instance)
        {
            s_StateBag._pool.Release();            
        }
    }
}
