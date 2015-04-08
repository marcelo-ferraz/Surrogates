
using Surrogates.Utilities.SDILReader;
using System;
using System.Reflection;

namespace Surrogates.Utilities.Mixins
{
    public static class PropGetterMixins
    {        
        public static Type GetPropType<T>(this Func<T, object> propGetter)
        {
            var reader =
                new MethodBodyReader(propGetter.Method);

            string propName = null;

            for (int i = 0; i < reader.Instructions.Count; i++)
            {
                var code =
                    reader.Instructions[i].Code.Name;

                if (code != "callvirt" && code != "call")
                { continue; }

                if (!(reader.Instructions[1].Operand is MethodInfo))
                { continue; }

                var operand = 
                    (MethodInfo)reader.Instructions[i].Operand;

                propName = operand.Name;

                if (!propName.Contains("get_") && !propName.Contains("set_"))
                { throw new ArgumentException("What was provided is not an property"); }

                return propName.StartsWith("get_") ?
                    operand.ReturnType :
                    operand.GetParameters()[0].ParameterType;
            }
            return null;
        }
    }
}
