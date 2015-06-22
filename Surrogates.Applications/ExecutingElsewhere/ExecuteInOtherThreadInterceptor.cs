using System;
using System.Threading.Tasks;

namespace Surrogates.Applications.ExecutingElsewhere
{
    public class ExecuteInOtherThreadInterceptor
    {
        internal object Execute(Delegate s_method, object[] s_args, bool s_Forget)
        {
            object result = null;

            var task = Task.Factory.StartNew(
                () => result = s_method.DynamicInvoke(s_args));

            if (!s_Forget) { task.Wait(); }

            return result;
        }
    }
}
