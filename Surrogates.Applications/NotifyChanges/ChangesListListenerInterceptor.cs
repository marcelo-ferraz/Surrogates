using Surrogates.Utilities;
using System;
using System.Collections.Generic;

namespace Surrogates.Applications.NotifyChanges
{
    public class ChangesListListenerInterceptor<I> //: InitiableInterceptor        
        where I : class
    {
        internal void Set<L>(BaseContainer4Surrogacy s_container, Delegate s_method, object[] s_args, I item, Action<L, I, object> s_Notifier, L s_instance)
            where L : class, ICollection<I>
        {     
            var newItem = s_container.Invoke<I>(
                stateBag: new
                {
                    Notifier = new Action<I, object>(
                        (i, v) =>
                            s_Notifier(s_instance, i, v))
                });

            s_args[s_args.Length - 1] =
                Clone.IntoTheSecond(item, newItem);
            
            s_method.DynamicInvoke(s_args);
        }
    }
}
