using System;
using System.Threading;

namespace Surrogates.Aspects.NotifyChanges
{
    public class InitiableInterceptor
    {
        private object _sync = new object();
        private bool _wasInit = false;

        protected void Try2Initate(Action init)
        {
            try
            {
                if (Monitor.TryEnter(this._sync, 500))
                {
                    if (this._wasInit) { return; }

                    init();
                }
            }
            finally
            { Monitor.Exit(this._sync); }
        }
    }

}
