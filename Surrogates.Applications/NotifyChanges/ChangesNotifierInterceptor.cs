using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime;

namespace Surrogates.Applications.NotifyChanges
{
    public class ChangesNotifierInterceptor<T> : NotifierBeforeAndAfter<T>
    {
        public override event Action<T, object> Before;

        public override event Action<T, object> After;

        public void Set(dynamic p_StateBag, Delegate p_Before, Delegate p_After, T s_instance, Delegate s_method, object s_value)
        {            
            // global list call
            if (p_StateBag != null)
            { Call(p_StateBag.Before, s_instance, s_value); }
            
            // global call
            Call(p_Before, s_instance, s_value); 

            // instance call
            Call(this.Before, s_instance, s_value); 

            s_method.DynamicInvoke(s_value);
            
            // global list call
            if (p_StateBag != null)
            { Call(p_StateBag.After, s_instance, s_value); }
            
            // global call
            Call(p_After, s_instance, s_value); 

            // instance call
            Call(this.After, s_instance, s_value); 
        }
    }
}