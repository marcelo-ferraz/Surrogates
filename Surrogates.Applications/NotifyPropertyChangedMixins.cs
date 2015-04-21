using Surrogates.Expressions;
using Surrogates.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Surrogates.Applications
{
    public static class NotifyChangesMixins
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

        public class ChangesNotifierInterceptor<T> : InitiableInterceptor
        {
            public event Action<T, object> Changed;

            public void Set(dynamic s_StateBag, T s_instance, Delegate s_method, object s_value)
            {
                Try2Initate(() => 
                    this.Changed += s_StateBag.Notifier);

                s_method.DynamicInvoke(s_value);

                if (this.Changed != null)
                { this.Changed(s_instance, s_value); }
            }
        }

        public class ChangesListListenerInterceptor<L, I>: InitiableInterceptor
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

        public static AndExpression<L> NotifyChanges<L, I>(this ApplyExpression<L> that, Action<L, I, object> listener)
            where L : class, IList<I> 
            where I : class
        {
            var ext = new ShallowExtension<L>();
            Pass.On(that, ext);

            return ext
                .Factory
                .Replace
                .Method("set_Item")
                .Using<ChangesListListenerInterceptor<L, I>>("Set")
                .And
                .AddProperty<Action<L, I, object>>("Notifier", listener);
        }
    }
}
