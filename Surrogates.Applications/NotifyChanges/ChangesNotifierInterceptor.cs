using System;

namespace Surrogates.Applications.NotifyChanges
{
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

}
