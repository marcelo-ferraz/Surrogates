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
            bool hasThisDynamic_ = false;
            bool hasArgs = false;
           
            foreach (var param in baseMethod.GetParameters())
            {
                builder.DefineParameter(++pIndex, ParameterAttributes.None, param.Name);
            } 
            
            foreach (var param in strat.Interceptor.Method.GetParameters())
            {
                if (param.Name == "_" && param.ParameterType == typeof(object))
                { hasThisDynamic_ = true; }

                if (param.ParameterType != typeof(object[]) && param.Name != "s_arguments" && param.Name != "s_args")
                { hasArgs = true; }
            }

            if (hasArgs)
            { strat.Interceptor.ArgsLocal = gen.DeclareLocal(typeof(object[])); }

            if (hasThisDynamic_)
            { strat.Interceptor.ThisDynamic_Local = gen.DeclareLocal(typeof(object[])); }

            returnField = baseMethod.ReturnType != typeof(void) ?
                gen.DeclareLocal(baseMethod.ReturnType) :
                null;

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