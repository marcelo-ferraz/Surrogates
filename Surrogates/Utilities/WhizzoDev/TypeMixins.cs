using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Surrogates.Utilities.WhizzoDev
{
    public static class TypeMixins
    {
        public static MethodInfo GetToListOfMethod(this Type type, Type sourceType, Type destType)
        {
            return GetMethod(
                type,
                "ToListOf",
                sourceType,
                destType);
        }

        public static MethodInfo GetToArrayOfMethod(this Type type, Type sourceType, Type destType)
        {
            return GetMethod(
                type,
                "ToArrayOf",
                sourceType,
                destType);
        }

        public static MethodInfo GetMethod(this Type type,
            string name,
            Type sourceType,
            Type destType)
        {
            foreach (var method in type.GetMethods())
            {
                if (method.Name != name) { continue; }

                if (!method.IsGenericMethod && destType != null) { continue; }

                if (sourceType == method.GetParameters()[0].ParameterType || method.GetParameters()[0].ParameterType.IsAssignableFrom(sourceType))
                {
                    return method.MakeGenericMethod(new[] { destType });
                }
            }
            return null;
        }
    }
}
