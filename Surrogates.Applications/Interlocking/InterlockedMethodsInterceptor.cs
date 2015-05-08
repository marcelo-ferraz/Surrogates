using System;
using System.Threading;

namespace Surrogates.Applications.Interlocking
{
    public class InterlockedFuncInterceptor
        : InterlockedMethodInterceptor
    {
        public object Read(Delegate s_method, object[] s_arguments)
        {
            object ret = null;
            bool lockWasHeld = false;
            try
            {
                try { }
                finally
                {
                    lockWasHeld = Lock.TryEnterReadLock(500);
                }

                if (lockWasHeld) { ret = s_method.DynamicInvoke(s_arguments); }
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

        //public void Write(object[] s_arguments, Delegate s_method)
        public void Write(dynamic _)
        {
            bool lockWasHeld = false;
            try
            {
                try { }
                finally
                {
                    if (Lock.IsWriteLockHeld)
                    { Lock.ExitWriteLock(); }

                    lockWasHeld = Lock.TryEnterWriteLock(500);
                }

                if (lockWasHeld) { _.Caller.DynamicInvoke(_.Arguments); }
            }
            finally
            {
                if (lockWasHeld)
                { Lock.ExitWriteLock(); } 
            }
        }
    }
}
