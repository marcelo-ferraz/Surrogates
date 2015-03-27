
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Utilities.Mixins
{
    public static class TypeMixins
    {
        public static MethodInfo GetMethod4Surrogacy(this Type self, string name)
        {
            Func<BindingFlags, MethodInfo> get = flags =>
                self.GetMethod(name, BindingFlags.Instance | flags);
            
            MethodInfo method;

            if ((method = get(BindingFlags.NonPublic)) == null)
            {
                if ((method = get(BindingFlags.Public)) == null)
                {
                    throw new KeyNotFoundException(string.Format(
                        "The method '{0}' wans not found withn the type '{1}'", name, self.Name));
                }
            }
            return method;
        }

        public static PropertyInfo GetProp4Surrogacy(this Type self, string name)
        {
            Func<BindingFlags, PropertyInfo> get = flags =>
                self.GetProperty(name, BindingFlags.Instance | flags);

            PropertyInfo prop;

            if ((prop = get(BindingFlags.NonPublic)) == null)
            {
                if ((prop = get(BindingFlags.Public)) == null)
                {
                    throw new KeyNotFoundException(string.Format(
                        "The property '{0}' wans not found withn the type '{1}'", name, self.Name));
                }
            }
            return prop;
        }
    }
}
