using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.LazyLoading
{
    public class LazyLoadingInterceptor<T, TProp>
    {
        public class PropInfo
        {
            public string Name { get; set; }

            public TProp Value;

            public bool IsDirty = false;
        }

        private static IDictionary<string, PropInfo> _interceptors;

        public IDictionary<string, PropInfo> Interceptors
        {
            get { return _interceptors; }
        }

        static LazyLoadingInterceptor()
        {
            _interceptors = 
                new Dictionary<string, PropInfo>();
        }

        private static PropInfo GetPropInfo(string s_name)
        {
            if(!_interceptors.ContainsKey(s_name))
            {
                var prop =                     
                    new PropInfo {  
                        IsDirty = false,
                        Name = s_name
                    };

                _interceptors.Add(s_name, prop);
                return prop;
            }

            return _interceptors[s_name];
        }


        public TProp Load(string s_name, T s_instance, Dictionary<string, Func<string, T, TProp>> s_Loaders)
        {
            var prop =
                GetPropInfo(s_name);

            return object.Equals(prop.Value, default(TProp)) ?
                (prop.Value = s_Loaders[s_name](s_name, s_instance)) :
                prop.Value;
        }

        public void MarkAsDirty(string s_name, T s_instance, TProp s_value)
        {
            var prop =
                GetPropInfo(s_name);

            prop.IsDirty = true;
            prop.Value = s_value;
        }
    }
}
