using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Applications.LazyLoading
{
    public class LazyLoadingInterceptor<T> : ILazyLoadingInterceptor
    {
        private static IDictionary<string, LazyProperty> _properties;

        /// <summary>
        /// A list of the watched properties by this interceptor
        /// </summary>
        public IDictionary<string, LazyProperty> Properties
        {
            get { return _properties; }
        }

        static LazyLoadingInterceptor()
        {
            _properties =
                new Dictionary<string, LazyProperty>();
        }

        private static LazyProperty GetPropInfo(string s_name)
        {
            if(!_properties.ContainsKey(s_name))
            {
                var prop =
                    new LazyProperty
                    {  
                        IsDirty = false,
                        Name = s_name
                    };

                _properties.Add(s_name, prop);
                return prop;
            }

            return _properties[s_name];
        }

        /// <summary>
        /// Invokes the supplied loader for the properties if needed. 
        /// </summary>
        /// <param name="s_name">the name of the properties</param>
        /// <param name="s_instance">the instance that holds that properties</param>
        /// <param name="s_Loaders">the loader collection that will contain the loader for such properties</param>
        /// <returns>the value of that properties</returns>
        public object Load(string s_name, T s_instance, Dictionary<string, Func<string, T, object>> p_Loaders)
        {
            var prop =
                GetPropInfo(s_name);

            return prop.Value == null ?
                (prop.Value = p_Loaders[s_name](s_name, s_instance)) :
                prop.Value;
        }

        /// <summary>
        /// It marks that properties as 'dirty', meaning that it was modified by other than the lazyloading feature
        /// </summary>
        /// <param name="s_name">the name of the properties</param>
        /// <param name="s_instance">the instance that holds that properties</param>
        /// <param name="s_value">the value that is going to e assigned to that properties</param>
        public void MarkAsDirty(string s_name, T s_instance, object s_value)
        {
            var prop =
                GetPropInfo(s_name);

            prop.IsDirty = true;
            prop.Value = s_value;
        }
    }
}
