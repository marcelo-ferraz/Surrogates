using System.Threading;

namespace Surrogates.Applications.Interlocking
{
    public abstract class InterlockedPropertyInterceptor
    {
        private ReaderWriterLockSlim _lock;
        protected object _field;
        public InterlockedPropertyInterceptor() 
        {
            _lock = new ReaderWriterLockSlim();
        }

        ~InterlockedPropertyInterceptor()
        {
            _lock.Dispose();
        }

        protected abstract object GetField(object field);

        public object Get()
        {            
            object ret = null;
            bool lockWasHeld = false;
            try
            {
                try { } finally
                {
                    lockWasHeld = _lock.TryEnterReadLock(500);
                }

                if (lockWasHeld) { ret = GetField(_field); }
            }
            finally
            {
                if (lockWasHeld) { _lock.ExitReadLock(); }                               
            }
            return ret;
        }

        public void Set(object s_value)
        {            
            bool lockWasHeld = false;
            try
            {
                try { } finally
                { lockWasHeld = _lock.TryEnterWriteLock(500); }

                if (lockWasHeld) { _field = s_value; }
            }
            finally
            { 
                if (lockWasHeld) { _lock.ExitWriteLock(); } 
            }
        }
    }
}