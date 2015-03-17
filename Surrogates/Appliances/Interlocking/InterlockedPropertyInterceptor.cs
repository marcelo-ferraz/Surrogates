using System.Threading;

namespace Surrogates.Appliances
{
    public abstract class InterlockedPropertyInterceptor<T>
    {
        ReaderWriterLockSlim _lock;

        public InterlockedPropertyInterceptor() 
        {
            _lock = new ReaderWriterLockSlim();
        }

        ~InterlockedPropertyInterceptor()
        {
            _lock.Dispose();
        }

        protected abstract T GetField(T field);        

        public T Get(T s_field)
        { 
            T ret = default(T);
            bool lockWasHeld = false;
            try
            {
                try { } finally
                {
                    lockWasHeld = _lock.TryEnterReadLock(500);
                }

                if (lockWasHeld) { ret = GetField(s_field); }
            }
            finally
            {
                if (lockWasHeld) { _lock.ExitReadLock(); }                               
            }
            return ret;
        }

        public void Set(ref T s_field, T s_value)
        {
            bool lockWasHeld = false;
            try
            {
                try { } finally
                { lockWasHeld = _lock.TryEnterReadLock(500); }

                if (lockWasHeld) { s_field = s_value; }
            }
            finally
            { 
                if (lockWasHeld) { _lock.ExitWriteLock(); } 
            }
        }
    }
}