using Surrogates.Model;
using System;
using System.Reflection;

namespace Surrogates.Utilities
{
    public static class Infer
    {
        public static bool IsAutomatic(SurrogatedProperty prop)
        {
            var owner = 
                prop.Original.ReflectedType;

            var field = owner.GetField(
                string.Format("<{0}>k__BackingField", prop.Original.Name), 
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            return field != null;
        }
        
        /// <summary>
        /// Infers a delegate type from the method passed, so it can be compared to the parameter
        /// </summary>
        /// <param name="baseMethod"></param>
        /// <returns></returns>
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
