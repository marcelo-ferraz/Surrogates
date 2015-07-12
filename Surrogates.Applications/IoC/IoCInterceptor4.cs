using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.IoC
{
    public class IoCInterceptor4<T>
    {
        private T _value;

        public T Get(string s_name, Dictionary<string, object[]> s_Params)
        {
            var isDefault =
                object.ReferenceEquals(_value, default(T));

            if (!isDefault) { return _value; }

            if (s_Params == null)
            { return _value = Activator.CreateInstance<T>(); }

            var args = s_Params[s_name];

            return args != null && args.Length > 0 ?
                _value = (T)Activator.CreateInstance(typeof(T), args) :
                _value = Activator.CreateInstance<T>();
        }

        public void Set(T s_value)
        {
            _value = s_value;
        }
    }
}
