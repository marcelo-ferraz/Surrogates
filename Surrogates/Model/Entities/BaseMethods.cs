
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
   
        public void Add(MethodInfo method, Strategy.ForMethods current)
        {
            foreach (var arg in method.GetParameters())
            {
                if (!arg.ParameterType.IsAssignableFrom(typeof(Delegate)))
                { continue; }

                MethodInfo paramMethod;

                if (arg.Name.ToLower() == "s_method" || arg.Name == "_")
                {
                    foreach (var m in current.Methods)
                    { _methods.Add(m); }

                    continue;
                }

                paramMethod = current
                    .BaseType
                    .GetMethod4Surrogacy(arg.Name.Length < 2 ? arg.Name : arg.Name.Substring(2), null, false);

                if (paramMethod == null) { continue; }

                _methods.Add(paramMethod);                
            }
        }

        public IEnumerator<MethodInfo> GetEnumerator()
        {
            if (this.Field == null)
            { 
                throw new NotSupportedException("In order to iterate throug the methods, a field for them, at the new type ought to be created"); 
            }

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
