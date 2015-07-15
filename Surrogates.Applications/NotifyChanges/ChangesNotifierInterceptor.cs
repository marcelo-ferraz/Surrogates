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

        public void Set(dynamic s_StateBag, Delegate s_Before, Delegate s_After, T s_instance, Delegate s_method, object s_value)
        {
            var bag = s_StateBag as IDictionary<String, object>;
            
            // global list call
            if (s_StateBag != null && bag.ContainsKey("Before")) 
            { Call(s_StateBag.Before, s_instance, s_value); }
            
            // global call
            Call(s_Before, s_instance, s_value); 

            // instance call
            Call(this.Before, s_instance, s_value); 

            s_method.DynamicInvoke(s_value);
            
            // global list call
            if (s_StateBag != null && bag.ContainsKey("After"))
            { Call(s_StateBag.After, s_instance, s_value); }
            
            // global call
            Call(s_After, s_instance, s_value); 

            // instance call
            Call(this.After, s_instance, s_value); 
        }
    }
}