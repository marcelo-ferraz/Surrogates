using System;
using System.Collections.Generic;

namespace Surrogates.Applications.Contracts
{
    public class ContractsInterceptor<T>
    {
        public object ValidateBeforeExecute(string s_name, Delegate s_method, object[] s_args, Dictionary<string, Action<object[]>> s_PreValidators)
        {
            s_PreValidators[s_name](s_args);

            return s_method.DynamicInvoke(s_args);
        }
    }
}
