using Surrogates.Tactics;
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

        public static bool Is4SelfMethod(this ParameterInfo self, MethodInfo method)
        {
            var nameMatches = 
                !string.IsNullOrEmpty(self.Name) && 
                self.Name.Length > 1 && 
                self.Name.Substring(0, 2) == "s_" &&
                self.Name.ToLower().Substring(2) == method.Name.ToLower();

            return (self.Name == "s_method" || nameMatches)
                && typeof(Delegate).IsAssignableFrom(self.ParameterType);
        }

        public static bool Is4AnyField(this ParameterInfo self)
        {
            return self.Name.StartsWith("f_");
        }

        public static bool Is4anyProperty(this ParameterInfo self)
        {
            return self.Name.StartsWith("p_");
        }

        public static bool Is4SelfArguments(this ParameterInfo self)
        {
            return self.ParameterType == typeof(object[]) && (self.Name == "s_arguments" || self.Name == "s_args");
        }

        public static bool IsDynamic_(this ParameterInfo self)
        {
            return self.Name == "_" && self.ParameterType == typeof(object);
        }

        public static bool Is4SomeMethod(this ParameterInfo self)
        {
            return self.Name.ToLower().StartsWith("m_") && typeof(Delegate).IsAssignableFrom(self.ParameterType);
        }

        public static bool Is4Instance(this ParameterInfo param, Type baseType)
        {
            return baseType.IsAssignableFrom(param.ParameterType) && (param.Name == "s_instance" || param.Name == "s_holder");
        }
    }
}
