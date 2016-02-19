using Surrogates.Model.Entities;
using Surrogates.Tactics;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utilities.Mixins
{
    public static class StrategyMixins
    {
        internal static OverridenMethod Override(this Strategy strat, Strategy.InterceptorInfo interceptor, MethodInfo baseMethod, Func<OverridenMethod, ParameterInfo, int, bool> emitParameters)
        {
            var overriden = 
                new OverridenMethod();

            var attrs = MethodAttributes.Virtual;

            FieldBuilder field = null;
            if (interceptor.DeclaredType != null)
            {
                field = strat.Fields
                     .Get(interceptor.DeclaredType, interceptor.Name);
            }

            if (baseMethod.Attributes.HasFlag(MethodAttributes.Public))
            { attrs |= MethodAttributes.Public; }

            if (baseMethod.Attributes.HasFlag(MethodAttributes.FamANDAssem))
            { attrs |= MethodAttributes.FamANDAssem; }

            overriden.Builder = strat.TypeBuilder.DefineMethod(
                baseMethod.Name,
                attrs,
                baseMethod.ReturnType,
                baseMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            overriden.Generator =
                overriden.Builder.GetILGenerator();

            int pIndex = 0;
            foreach (var param in baseMethod.GetParameters())
            {
                overriden.Builder.DefineParameter(++pIndex, ParameterAttributes.None, param.Name);
            }

            overriden.Return = SetLocals4
                .AllComplexParameters(strat, interceptor, baseMethod, overriden);

            if (field != null)
            {
                overriden.Generator.Emit(OpCodes.Ldarg_0);
                overriden.Generator.Emit(OpCodes.Ldfld, field);
            }

            var @params = overriden.Generator.EmitParameters(
                strat,
                interceptor,
                overriden,
                baseMethod,
                (p, i) => emitParameters(overriden, p, i));

            overriden.Generator.EmitCall(
                interceptor.Method, @params);
            
            return overriden;
        }
    }
}