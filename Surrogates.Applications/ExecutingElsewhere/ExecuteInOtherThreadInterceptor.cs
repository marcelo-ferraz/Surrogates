using Surrogates.Aspects.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Surrogates.Aspects.ExecutingElsewhere
{
    public class ExecuteInOtherThreadInterceptor : Locked
    {
        public object Execute(Delegate s_method, object[] s_args, Dictionary<IntPtr, bool> p_Forget, Dictionary<IntPtr, Task<object>> p_Tasks)
        {
            object result = null;
                        
            var handle = 
                s_method.Method.MethodHandle.Value;

            var task = Task.Factory.StartNew<object>(
                () =>
                { return result = s_method.DynamicInvoke(s_args); });

            Lock(p_Tasks, t => t[handle] = task);

            if (!p_Forget[handle]) { task.Wait(); }
            
            return result;
        }
    }
}
