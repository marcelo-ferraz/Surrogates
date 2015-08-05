using System;
using System.Threading;

namespace Surrogates.Aspects.Utilities
{
    public abstract class Locked
    {
        protected int Timeout = 500;

        static Locked()
        { }
                
        public void Lock<T>(T obj, Action<T> action)
        {
            bool wasLocked = false;
            try
            {
                try { } finally
                {
                    if (wasLocked = Monitor.TryEnter(obj, this.Timeout))
                    { action(obj); }
                }
            }
            finally
            {
                if (wasLocked)
                { Monitor.Exit(obj); }
            }
        }
    }
}
