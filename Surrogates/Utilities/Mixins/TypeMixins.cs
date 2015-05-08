
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Utilities.Mixins
{
    public static class TypeMixins
    {
        public static MethodInfo GetMethod4Surrogacy(this Type self, string name, Type[] parameterTypes = null, bool throwExWhenNotFound = true)
        {
            MethodInfo method = null;
            
            try
            {
                Func<BindingFlags, MethodInfo> get =
                    parameterTypes == null || parameterTypes.Length < 1 ?
                    (Func<BindingFlags, MethodInfo>)
                    (flags => self.GetMethod(name, BindingFlags.Instance | flags)) :
                    (flags => self.GetMethod(name, BindingFlags.Instance | flags, null, parameterTypes, null));

                if ((method = get(BindingFlags.NonPublic)) == null)
                {
                    if ((method = get(BindingFlags.Public)) == null && throwExWhenNotFound)
                    {
                        throw new KeyNotFoundException(string.Format(
                            "The method '{0}' wans not found withn the type '{1}'", name, self.Name));
                    }
                }
            }
            catch (AmbiguousMatchException ex)
            {
                foreach (var m in self.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (name == m.Name)
                    {
                        method = m;
                        break;
                    }
                }

                foreach (var m in self.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
                {
                    if (name == m.Name)
                    {
                        method = m;
                        break;
                    }
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
