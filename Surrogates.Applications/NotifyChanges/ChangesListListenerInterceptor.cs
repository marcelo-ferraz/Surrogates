using Surrogates.Utilities;
using Surrogates.Utilities.WhizzoDev;
using System;
using System.Collections.Generic;

namespace Surrogates.Applications.NotifyChanges
{
    public class ChangesListListenerInterceptor<I> //: InitiableInterceptor        
        where I : class
    {
        public void Set<L>(BaseContainer4Surrogacy s_Container, Delegate s_method, object[] s_args, I item, Action<L, I, object> s_Notifier, L s_instance)
            where L : class, ICollection<I>
        {
            var newItem = s_Container.Invoke<I>();
                //stateBag: new
                //{
                //    Notifier = new Action<I, object>(
                //        (i, v) =>
                //            s_Notifier(s_instance, i, v))
                //}
                //);

            s_args[s_args.Length - 1] =
                CloneHelper.Merge(item, newItem);
                
                //CloneHelper.Merge(item, newItem);

            System.Diagnostics.Debugger.Break();

            s_method.DynamicInvoke(s_args);
        }
    }
}
