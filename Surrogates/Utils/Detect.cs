using System;
using System.Reflection;
using Surrogates.Mappers;

namespace Surrogates.Utils
{
    public static class Infer
    {
        public static bool IsAutomatic(Property prop)
        {
            var owner = 
                prop.Original.ReflectedType;

            var field = owner.GetField(
                string.Format("<{0}>k__BackingField", prop.Original.Name), 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            return field != null;
        }


        public static Type DelegateTypeFrom(MethodInfo baseMethod)
        {
            var isFunc =
                baseMethod.ReturnType != typeof(void);

            var baseParams =
                baseMethod.GetParameters();
            
            Type[] paramTypes = (Type[])Array.CreateInstance(typeof(Type),
                isFunc ?
                baseParams.Length + 1 :
                baseParams.Length);

            for (int i = 0; i < baseParams.Length; i++)
            { paramTypes[i] = baseParams[i].ParameterType; }

            Type delType;

            if (isFunc)
            {
                paramTypes[paramTypes.Length - 1] = baseMethod.ReturnType;
                delType = Type
                    .GetType(string.Concat("System.Func`", (paramTypes.Length).ToString()))
                    .MakeGenericType(paramTypes);
            }
            else if (paramTypes.Length > 0)
            {
                delType = Type
                    .GetType(string.Concat("System.Action`", paramTypes.Length.ToString()))
                    .MakeGenericType(paramTypes);
            }
            else
            { delType = typeof(Action); }

            return delType;
        }

    }
}
