using Surrogates.Model;
using Surrogates.Model.Collections;
using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;

namespace Surrogates.Executioners
{
    public class ReplaceExecutioner : Executioner
    {
        protected override void OverrideWithAction(MethodInfo baseAction, Strategy strategy, Strategy.InterceptorInfo interceptor, OverridenMethod overriden)
        {
            if (overriden.Return != null)
            { overriden.Generator.EmitDefaultLocalValue(baseAction.ReturnType); }

            overriden.Generator.Emit(OpCodes.Ret);
        }

        protected override void OverrideWithFunction(MethodInfo baseFunction, Strategy strategy, Strategy.InterceptorInfo interceptor, OverridenMethod overriden)
        {
            //the base method is void, discard the value
            if (overriden.Return == null)
            {
                overriden.Generator.Emit(OpCodes.Pop);
            }
            else if (!interceptor.Method.ReturnType.IsAssignableFrom(baseFunction.ReturnType))
            {
                overriden.Generator.EmitDefaultLocalValue(interceptor.Method.ReturnType);
            }
            else // in case the new method's return needs to be cast 
                if (interceptor.Method.ReturnType != baseFunction.ReturnType)
                {
                    if (baseFunction.ReturnType.IsValueType)
                    {
                        overriden.Generator.Emit(OpCodes.Unbox_Any, baseFunction.ReturnType);
                        overriden.Generator.Emit(OpCodes.Stloc, overriden.Return);
                        overriden.Generator.Emit(OpCodes.Ldloc, overriden.Return);
                    }
                    else
                    {
                        overriden.Generator.Emit(OpCodes.Castclass, baseFunction.ReturnType);
                    }
                }

            overriden.Generator.Emit(OpCodes.Ret);
        }
    }
}