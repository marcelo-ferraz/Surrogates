using System;

namespace Surrogates.Applications.NotifyChanges
{
    public class ChangesNotifierInterceptor<T> : InitiableInterceptor
    {
        public void Set(dynamic s_StateBag, T s_instance, Delegate s_method, object s_value)
        {
            if (s_StateBag != null && s_StateBag.Before != null)
            {
                ((Action<T, object>)s_StateBag.Before).Invoke(s_instance, s_value);
            }

            s_method.DynamicInvoke(s_value);

            if (s_StateBag != null && s_StateBag.After != null)
            {
                ((Action<T, object>)s_StateBag.After).Invoke(s_instance, s_value);
            }
        }
    }
}