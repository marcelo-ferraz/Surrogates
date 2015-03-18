using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Surrogates.Utilities.Mixins;

namespace Surrogates.Executioners
{
    public class DisableExecutioner : Executioner
    {
        protected void DisableAction(MethodInfo method, Strategy.ForMethods strategy)
        {
            
        }

        protected void DisableFunction(MethodInfo method, Strategy.ForMethods strategy)
        {
            
        }

        public override void Execute4Properties(Strategy.ForProperties strategy)
        {

        }

        public override void Execute4Methods(Strategy.ForMethods strategy)
        {
            foreach (var method in strategy.Methods)
            {
                var builder = strategy.TypeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType,
                method.GetParameters().Select(p => p.ParameterType).ToArray());

                var gen = builder.GetILGenerator();

                if (method.ReturnType != typeof(void))
                {
                    gen.EmitDefaultValue(method.ReturnType);
                }
                gen.Emit(OpCodes.Ret);
            }
        }
    }
}
