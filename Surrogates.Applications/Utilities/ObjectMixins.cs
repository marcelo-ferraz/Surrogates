using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Aspects.Utilities
{
    public static class ObjectMixins
    {
        public static bool Is<T>(this object self)
        {
            return self.Is(typeof(T));
        }

        public static bool Is(this object self, params Type[] types)
        {
            var compared = self is Type ?
                self as Type :
                self.GetType();

            foreach (var type in types)
            {
                if (compared.IsGenericType && type.IsGenericTypeDefinition)
                { return type.IsAssignableFrom(compared.GetGenericTypeDefinition()); }

                if (type.IsAssignableFrom(compared)) { return true; }
            }

            return false;
        }

        public static bool IsCollection(this ParameterInfo self)
        {
            return self.ParameterType.Is(
                typeof(ICollection),
                typeof(ICollection<>));
        }

        public static Func<object, object[], object> GetCount(this ParameterInfo self)
        {
            if (self.IsCollection())
            {
                return new Func<object, object[], object>(self.ParameterType.GetProperty("Count").GetValue);
            }

            if (typeof(Array).IsAssignableFrom(self.ParameterType) || typeof(string).IsAssignableFrom(self.ParameterType))
            {
                return new Func<object, object[], object>(self.ParameterType.GetProperty("Length").GetValue);
            }

            throw new NotSupportedException(
                string.Format("The parameter given, '{0}' is not a supported collection or array.", self.Name));
        }
    }
}