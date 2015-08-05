using System;
using System.Threading;

namespace Surrogates.Aspects.Interlocking
{
    public class InterlockedMethodInterceptor
    {
        private ReaderWriterLockSlim _lock;

        public InterlockedMethodInterceptor()
        {
            _lock = new ReaderWriterLockSlim();
        }

        ~InterlockedMethodInterceptor()
        {
            _lock.Dispose();
        }

        public object Read(Delegate s_method, object[] s_arguments)
        {
            object ret = null;
            bool lockWasHeld = false;
            try
            {
                try { }
                finally
                {
                    lockWasHeld = _lock.TryEnterReadLock(500);
                }

                if (lockWasHeld) { ret = s_method.DynamicInvoke(s_arguments); }
            }
            finally
            {
                if (lockWasHeld) { _lock.ExitReadLock(); }
            }
            return ret;
        }
        
        public void Write(Delegate s_method, object[] s_arguments)
        {
            bool lockWasHeld = false;
            try
            {
                try { }
                finally
                {
                    if (_lock.IsWriteLockHeld)
                    { _lock.ExitWriteLock(); }

                    lockWasHeld = _lock.TryEnterWriteLock(500);
                }

                if (lockWasHeld) { s_method.DynamicInvoke(s_arguments); }
            }
            finally
            {
                if (lockWasHeld)
                { _lock.ExitWriteLock(); } 
            }
        }
    }
}
