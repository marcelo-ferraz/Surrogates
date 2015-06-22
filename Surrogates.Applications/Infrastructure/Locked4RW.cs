using System;
using System.Threading;

namespace Surrogates.Applications.Infrastructure
{
    public abstract class Locked4RW
    {
        protected static ReaderWriterLockSlim Lock;

        protected int Timeout = 500;

        static Locked4RW()
        { Lock = new ReaderWriterLockSlim(); }

        public void Read(Action read)
        {
            bool isLocked = false;
            try
            {
                try { }
                finally
                {
                    if (isLocked = Lock.TryEnterReadLock(this.Timeout))
                    { read(); }
                }
            }
            finally
            {
                if (isLocked)
                { Lock.ExitReadLock(); }
            }
        }


        public void Write(Action read)
        {
            bool isLocked = false;
            try
            {
                try { }
                finally
                {
                    if (isLocked = Lock.TryEnterWriteLock(this.Timeout))
                    { read(); }
                }
            }
            finally
            {
                if (isLocked)
                { Lock.ExitWriteLock(); }
            }
        }
    }
}
