using System;
using System.Collections.Generic;
using System.Threading;

namespace Surrogates.Applications.Pooling
{
    public abstract class PoolInterceptor<T>
        where T : IDisposable
    {
        /// <summary>
        /// To be used alongside with the holder of the property
        /// </summary>
        public class ToGet : PoolInterceptor<T>
        {
            private bool _initiated;

            private object _sync;

            public ToGet()
            {
                this._sync = new object();
            }

            protected Pool<T> GetPool(dynamic bag, SurrogatesContainer s_Container)
            {
                bool lockAcquired = false;
                try
                {
                    try { }
                    finally
                    {
                        if (lockAcquired = (base.Pool == null && (Monitor.TryEnter(_sync, 500) && !this._initiated)))
                        {                            
                            base.Pool = new Pool<T>(
                                (int)bag.Size,
                                p => s_Container.Invoke<T>(stateBag: this),
                                (LoadingMode)bag.LoadingMode,
                                (AccessMode)bag.AccessMode);

                            this._initiated = true;
                        }
                    }
                }
                finally
                {
                    if (lockAcquired)
                    { Monitor.Exit(_sync); }
                }

                return Pool;
            }

            public T Get(SurrogatesContainer s_Container, string s_name, Dictionary<string, dynamic> s_PoolStates)
            {
                var bag = s_PoolStates[s_name];
                
                var result = 
                    GetPool(s_PoolStates[s_name], s_Container)
                    .Acquire();

                result.Pool = base.Pool;

                return result;
            }
        }

        /// <summary>
        /// To be used in the item, so that it can be properly released
        /// </summary>
        public class ToRelease: PoolInterceptor<T>
        {
            public void Dispose(T s_instance, Pool<T> s_Pool)
            {
                s_Pool.Release(s_instance);
            }
        }

        public class State
        { 
            public int PoolSize { get; set; }
            public LoadingMode LoadingMode { get; set; }
            public AccessMode AccessMode { get; set; }
        }

        protected Pool<T> Pool;
    }
}
