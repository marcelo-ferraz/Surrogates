
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Surrogates.Utilities.Mixins
{
    public static class TypeMixins
    {   
        public static MethodInfo GetMethod(this Type self, string name)
        {
            Func<BindingFlags, MethodInfo> get = flags =>
                self.GetMethod(name, BindingFlags.Instance | flags);
            
            MethodInfo method;

            if ((method = get(BindingFlags.NonPublic)) == null)
            {
                if ((method = get(BindingFlags.Public)) == null)
                {
                    throw new KeyNotFoundException(string.Format(
                        "The property '{0}' wans not found withn the type '{1}'", name, self.Name));
                }
            }
            return method;
        }

        public static PropertyInfo
    }
}
