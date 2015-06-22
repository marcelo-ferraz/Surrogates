using Surrogates.Utilities;
using System;
using System.Collections.Generic;

namespace Surrogates.Applications.NotifyChanges
{
    public class ChangesListListenerInterceptor<L, I> : InitiableInterceptor
        where L : class, IList<I>
        where I : class
    {
        internal void Set(BaseContainer4Surrogacy s_container, Action<L, I, object> s_Notifier, Delegate s_method, int index, I s_value, L s_instance)
        {
            Try2Initate(() =>
                s_container.Map(m =>
                    m.From<L>()
                    .Replace
                    .Properties(p => true) // all props
                    .Accessors(a =>
                        a.Setter.Using<ChangesNotifierInterceptor<L>>("Set"))));

            var val = s_container.Invoke<I>(
                stateBag: new
                {
                    Notifier = new Action<I, object>(
                        (item, v) =>
                            s_Notifier(s_instance, item, v))
                });

            val = Clone.IntoTheSecond(s_value, val);

            // call the original setter
            s_method.DynamicInvoke(index, val);
        }
    }

}
