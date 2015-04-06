using System;
using System.Threading;

namespace Surrogates.Applications.Interlocking
{
    public class InterlockedFuncInterceptor<T>
        : InterlockedMethodInterceptor
    {
        public T Read(Delegate s_method, object[] s_arguments)
        {
            T ret = default(T);
            bool lockWasHeld = false;
            try
            {
                try { }
                finally
                {
                    lockWasHeld = Lock.TryEnterReadLock(500);
                }

                if (lockWasHeld) { ret = (T)s_method.DynamicInvoke(s_arguments); }
            }
            finally
            {
                if (lockWasHeld) { Lock.ExitReadLock(); }
            }
            return ret;
        }
    }
    public class InterlockedActionInterceptor
        : InterlockedMethodInterceptor
    {
        public void Read(Delegate s_method, object[] s_arguments)
        {
            bool lockWasHeld = false;
            try
            {
                try { }
                finally
                {
                    lockWasHeld = Lock.TryEnterReadLock(500);
                }

                if (lockWasHeld) { s_method.DynamicInvoke(s_arguments); }
            }
            finally
            {
                if (lockWasHeld) { Lock.ExitReadLock(); }
            }
        }
    }

    public class InterlockedMethodInterceptor
    {
        protected ReaderWriterLockSlim Lock;

        public InterlockedMethodInterceptor() 
        {
            Lock = new ReaderWriterLockSlim();
        }

        ~InterlockedMethodInterceptor()
        {
            Lock.Dispose();
        }
        
        public void Write(Delegate s_method, object[] s_arguments)
        {
            bool lockWasHeld = false;
            try
            {
                try { } finally
                { lockWasHeld = Lock.TryEnterReadLock(500); }

                if (lockWasHeld) { s_method.DynamicInvoke(s_arguments); }
            }
            finally
            { 
                if (lockWasHeld) { Lock.ExitWriteLock(); } 
            }
        }
    }
}
