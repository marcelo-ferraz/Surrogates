using System;
using System.Reflection;

namespace Surrogates.Utilities.Mixins
{
    public static class ParameterInfoMixins
    {
        public static bool Is4Name(this ParameterInfo self)
        {
            return self.ParameterType == typeof(string) && self.Name == "s_name";
        }

        public static bool IsSelfMethod(this ParameterInfo self, MethodInfo method)
        {
            return //(self.Name == "s_method" || self.Name.ToLower().Substring(2) == method.Name) 
                self.Name == "s_method"
                && typeof(MulticastDelegate).IsAssignableFrom(self.ParameterType);
        }

        public static bool IsSelfArguments(this ParameterInfo self)
        {
            return self.ParameterType == typeof(object[]) && (self.Name == "s_arguments" || self.Name == "s_args");
        }

        public static bool IsDynamic_(this ParameterInfo self)
        {
            return self.Name == "_" && self.ParameterType == typeof(object);
        }

        public static bool Is4SomeMethod(this ParameterInfo self)
        {
            return self.Name.ToLower().StartsWith("m_") && typeof(MulticastDelegate).IsAssignableFrom(self.ParameterType);
        }

        public static bool IsInstance(this ParameterInfo param, Type baseType)
        {
            return baseType.IsAssignableFrom(param.ParameterType) && (param.Name == "s_instance" || param.Name == "s_holder");
        }
    }
}
