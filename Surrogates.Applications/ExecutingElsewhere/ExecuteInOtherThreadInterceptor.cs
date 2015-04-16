using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Applications.ExecutingElsewhere
{
    public class ExecuteInOtherThreadInterceptor<T>
    {
        internal T Execute(Delegate s_method, object[] s_args, bool s_Forget)
        {
            object result = null;

            var task = Task.Factory.StartNew(
                () => result = s_method.DynamicInvoke(s_args));

            if (!s_Forget) { task.Wait(); }

            return result == null ?
                default(T) :
                (T) result;
        }
    }
}
