using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;

namespace Surrogates.Aspects.NotifyChanges
{
    public class NotifierBeforeAndAfter
    {        
        [TargetedPatchingOptOut("")]
        protected static void Call(Delegate @event, params object[] args)
        {
            if (@event != null)
            { @event.DynamicInvoke(args); }
        }
    }

    public abstract class NotifierBeforeAndAfter<L, I> : NotifierBeforeAndAfter
    {
        public abstract event Action<L, I, object> Before;
        public abstract event Action<L, I, object> After;
    }

    public abstract class NotifierBeforeAndAfter<T> : NotifierBeforeAndAfter
    {
        public abstract event Action<T, object> Before;
        public abstract event Action<T, object> After;

        internal bool IsTurnedOn { get; set; }
    }
}