using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Utilities.Mixins
{
    public static class StrategyMixins
    {
        internal static ILGenerator Override(this Strategy.ForProperties strat, MethodInfo baseMethod, out LocalBuilder returnField)
        {
            throw new Exception();
        }         

        internal static ILGenerator Override(this Strategy.ForMethods strat, MethodInfo baseMethod, out LocalBuilder returnField)
        {
            var attrs = MethodAttributes.Virtual;

            var field = strat.Fields
                .Get(strat.Interceptor.DeclaredType, strat.Interceptor.Name);

            if (baseMethod.Attributes.HasFlag(MethodAttributes.Public))
            { attrs |= MethodAttributes.Public; }

            if (baseMethod.Attributes.HasFlag(MethodAttributes.FamANDAssem))
            { attrs |= MethodAttributes.FamANDAssem; }

            var builder = strat.TypeBuilder.DefineMethod(
                baseMethod.Name,
                attrs,
                baseMethod.ReturnType,
                baseMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            var gen = builder.GetILGenerator();

            int pIndex = 0;           
            foreach (var param in baseMethod.GetParameters())
            {
                builder.DefineParameter(++pIndex, ParameterAttributes.None, param.Name);
            }

            returnField = Initialize
                .AllComplexParameters(strat, baseMethod, gen);
            
            //gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, field); 

            var @params = gen.EmitParameters(
                strat,
                strat.Interceptor,
                baseMethod,
                (p, i) => 
                    gen.EmitArgumentsBasedOnOriginal(baseMethod, p, i, strat.BaseMethods.Field));
                   
            gen.EmitCall(strat.Interceptor.Method, @params);

            return gen;
        }
    }
}