using System;
using System.Collections.Generic;

namespace Surrogates.Aspects.Contracts
{
    public class ContractsInterceptor<T>
    {
        public object ValidateBeforeExecute(Delegate s_method, object[] s_args, Dictionary<IntPtr, Action<object[]>> p_PreValidators)
        {
            var handle = 
                s_method.Method.MethodHandle.Value;

            p_PreValidators[handle](s_args);

            return s_method.DynamicInvoke(s_args);
        }
    }
}
