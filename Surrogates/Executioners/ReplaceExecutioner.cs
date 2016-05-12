using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Executioners
{
    public class ReplaceExecutioner : Executioner
    {
        protected override void OverrideWithAction(MethodInfo baseAction, Strategy strategy, Strategy.InterceptorInfo interceptor, OverridenMethod overriden)
        {
            if (overriden.Return != null)
            { overriden.Generator.EmitDefaultValue(baseAction.ReturnType); }

            overriden.Generator.Emit(OpCodes.Ret);
        }

        protected override void OverrideWithFunction(MethodInfo baseFunction, Strategy strategy, Strategy.InterceptorInfo interceptor, OverridenMethod overriden)
        {
            //the base method is void, discard the value
            if (overriden.Return == null)
            {
                overriden.Generator.Emit(OpCodes.Pop);
            }
            else
            {
                bool isAssignable = baseFunction.ReturnType.IsAssignableFrom(interceptor.Method.ReturnType) ||
                    interceptor.Method.ReturnType.IsAssignableFrom(baseFunction.ReturnType);
                
                if (!isAssignable)
                {
                    overriden.Generator.EmitDefaultValue(interceptor.Method.ReturnType);
                }
                else // in case the new method's return needs to be cast 
                    if (interceptor.Method.ReturnType != baseFunction.ReturnType)
                    {
                        if (baseFunction.ReturnType.IsValueType)
                        {
                            var g = overriden.Generator;
                            var endOfFunction = g.DefineLabel();
                            var isNotNull = g.DefineLabel();

                            var returnLocal = 
                                g.DeclareLocal(baseFunction.ReturnType);
                            
                            g.Emit(OpCodes.Dup);
                            g.Emit(OpCodes.Brtrue_S, isNotNull);
                                                        
                            g.Emit(OpCodes.Pop);
                            g.EmitDefaultValue(baseFunction.ReturnType);
                            g.Emit(OpCodes.Box, baseFunction.ReturnType);

                            g.MarkLabel(isNotNull);
                            g.Emit(OpCodes.Unbox_Any, baseFunction.ReturnType);
                            g.Emit(OpCodes.Stloc, overriden.Return);
                            g.Emit(OpCodes.Ldloc, overriden.Return);
                            g.Emit(OpCodes.Stloc, returnLocal);
                            g.Emit(OpCodes.Br_S, endOfFunction);

                            g.MarkLabel(endOfFunction);
                            g.Emit(OpCodes.Ldloc, returnLocal);
                        }
                        else
                        {
                            overriden.Generator.Emit(OpCodes.Castclass, baseFunction.ReturnType);
                        }
                    }
            }
            overriden.Generator.Emit(OpCodes.Ret);
        }
    }
}