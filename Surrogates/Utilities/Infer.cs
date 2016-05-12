using Surrogates.Model;
using Surrogates.Utilities.Mixins;
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
                baseMethod.ReturnType != TypeOf.Void;

            var baseParams =
                baseMethod.GetParameters();
            
            Type[] paramTypes = (Type[])Array.CreateInstance(TypeOf.Type,
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
            { delType = TypeOf.Action; }

            return delType;
        }

        public static Func<object, Delegate> Delegate(Type owner, string methodName)
        {
            Type delType = null;
            return Delegate(owner, methodName, ref delType);
        }

        public static Func<object, Delegate> Delegate(Type owner, string methodName, ref Type delType)
        {
            var method = owner
                .GetMethod4Surrogacy(methodName);

            var handle = method
                .MethodHandle
                .GetFunctionPointer();

            var type = delType ?? 
                (delType = DelegateTypeFrom(method));

            return ctx => (Delegate)
                Activator.CreateInstance(type, ctx, handle);
        }

        public static Func<object, Delegate> Delegate<T>(string methodName, ref Type delType)
        {
            return Delegate(typeof(T), methodName, ref delType);
        }

        public static Func<object, Delegate> Delegate<T>(string methodName)
        {
            Type delType = null;
            return Delegate(typeof(T), methodName, ref delType);
        }
    }
}
