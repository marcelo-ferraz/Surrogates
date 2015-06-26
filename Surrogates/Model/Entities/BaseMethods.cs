
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Model.Entities
{
    /// <summary>
    /// Used to store all the base methods that will be used as parameters by the strategies
    /// </summary>
    public class BaseMethods : IEnumerable<MethodInfo>
    {
        public BaseMethods()
        {
            _methods = new HashSet<MethodInfo>();
        }

        private HashSet<MethodInfo> _methods;

        public FieldInfo Field { get; set; }

        private void Add(MethodInfo method, Strategy current, IEnumerable<MethodInfo> methods)
        {
            foreach (var arg in method.GetParameters())
            {
                if (!typeof(MulticastDelegate).IsAssignableFrom(arg.ParameterType))
                { continue; }

                MethodInfo paramMethod;

                if (arg.Name.ToLower() == "s_method" || arg.Name == "_")
                {
                    foreach (var m in methods)
                    { _methods.Add(m); }

                    continue;
                }

                paramMethod = current
                    .BaseType
                    .GetMethod4Surrogacy(arg.Name.Length < 2 ? arg.Name : arg.Name.Substring(2), null, false);

                if (paramMethod != null)
                { _methods.Add(paramMethod); }
            }
        }

        private IEnumerable<MethodInfo> GetGettersNSetters(Strategy.ForProperties current)
        {
            for (int i = 0; i < current.Properties.Count; i++)
            {
                yield return current.Properties[i].Original.GetGetMethod(true);
                yield return current.Properties[i].Original.GetSetMethod(true);
            }
        }

        public void Add(MethodInfo method, Strategy.ForProperties current)
        {
            this.Add(method, current, GetGettersNSetters(current));
        }

        public void Add(MethodInfo method, Strategy.ForMethods current)
        {
            this.Add(method, current, current.Methods);
        }

        public IEnumerator<MethodInfo> GetEnumerator()
        {
            return _methods.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count 
        { 
            get { return _methods.Count; } 
        }
    }
}
