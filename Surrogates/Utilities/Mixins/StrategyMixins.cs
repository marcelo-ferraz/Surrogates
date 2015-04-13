using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Utilities.Mixins
{
    //public static class StrategyMixins
    //{
    //    private static bool Has(this MethodAttributes attrs, MethodAttributes other)
    //    {
    //        return (attrs | other) != other;
    //    }

    //    internal static ILGenerator EmitOverride(this Strategy.ForMethods strategy, MethodInfo baseMethod, out LocalBuilder returnField)            
    //    {
    //        var attrs = MethodAttributes.Virtual;

    //        if (baseMethod.Attributes.HasFlag(MethodAttributes.Public))
    //        { attrs |= MethodAttributes.Public; }

    //        if (baseMethod.Attributes.HasFlag(MethodAttributes.FamANDAssem))
    //        { attrs |= MethodAttributes.FamANDAssem; }

    //        var builder = strategy.TypeBuilder.DefineMethod(
    //            baseMethod.Name,
    //            attrs,
    //            baseMethod.ReturnType,
    //            baseMethod.GetParameters().Select(p => p.ParameterType).ToArray());

    //        var gen = builder.GetILGenerator();

    //        returnField = baseMethod.ReturnType != typeof(void) ?
    //            gen.DeclareLocal(baseMethod.ReturnType) :
    //            null;

    //        //gen.Emit(OpCodes.Nop);
    //        gen.Emit(OpCodes.Ldarg_0);
    //        gen.Emit(OpCodes.Ldfld, strategy.Fields.Get(
    //            strategy.Interceptor.DeclaredType, strategy.Interceptor.Name));

    //        var @params =
    //            gen.EmitParameters(strategy, strategy.Interceptor.Method);

    //        gen.EmitCall(strategy.Interceptor.Method, @params);

    //        return gen;
    //    }

    //    internal static ILGenerator EmitOverride(this Strategy.ForMethods strategy, MethodInfo baseMethod)
    //    {
    //        LocalBuilder @return = null;
    //        return strategy.EmitOverride(baseMethod, out @return);
    //    }
    //}
}
