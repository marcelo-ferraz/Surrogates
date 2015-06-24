using Surrogates.Model;
using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;

namespace Surrogates.Executioners
{

    public class VisitationExecutioner : Executioner
    {
        // pensar em uma forma de colocar a seguinte chamada: EmitPropertyNameAndFieldAndValue, da classe PropertyMixins.

        protected override void OverrideWithAction(MethodInfo baseAction, Strategy strategy, Strategy.InterceptorInfo interceptor, OverridenMethod overriden)
        {            
            overriden.Generator.Emit(OpCodes.Ldarg_0);

            var @params =
                overriden.Generator.EmitParametersForSelf(strategy, baseAction);

            overriden.Generator.Emit(OpCodes.Call, baseAction);
            overriden.Generator.Emit(OpCodes.Ret);
        }

        protected override void OverrideWithFunction(MethodInfo baseFunction, Strategy strategy, Strategy.InterceptorInfo interceptor, OverridenMethod overriden)
        {
            overriden.Generator.Emit(OpCodes.Pop);

            overriden.Generator.Emit(OpCodes.Ldarg_0);

            var @params =
                overriden.Generator.EmitParametersForSelf(strategy, baseFunction);

            overriden.Generator.Emit(OpCodes.Call, baseFunction);

            overriden.Generator.Emit(OpCodes.Ret);
        }
    }
}