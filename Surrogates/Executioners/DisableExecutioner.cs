using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Executioners
{
    public class DisableExecutioner : Executioner
    {
        protected static void DisableMethod(TypeBuilder typeBuilder, MethodInfo method)
        {
            var builder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    method.ReturnType,
                    method.GetParameters().Select(p => p.ParameterType).ToArray());

            var gen = builder.GetILGenerator();

            if (method.ReturnType != typeof(void))
            {
                gen.EmitDefaultLocalValue(method.ReturnType);
            }
            gen.Emit(OpCodes.Ret);
        }
                
        protected static void DisableSetter(Strategy.ForProperties strategy, Model.SurrogatedProperty property)
        {
            var setter =
                CreateSetter(strategy, property.Original);

            var gen = setter.GetILGenerator();
            gen.Emit(OpCodes.Ret);

            property.Builder.SetSetMethod(setter);
        }
        
        protected static void DisableGetter(Strategy.ForProperties strategy, Model.SurrogatedProperty property)
        {
            var getter =
                CreateGetter(strategy, property.Original);

            var gen = getter.GetILGenerator();

            gen.EmitDefaultLocalValue(property.Original.PropertyType);
            gen.Emit(OpCodes.Ret);

            property.Builder.SetGetMethod(getter);
        }

        public override void Execute4Properties(Strategy.ForProperties strategy)
        {
            foreach (var property in strategy.Properties)
            {
                DisableSetter(strategy, property);

                DisableGetter(strategy, property);
            }
        }

        public override void Execute4Methods(Strategy.ForMethods strategy)
        {
            foreach (var method in strategy.Methods)
            {
                DisableMethod(strategy.TypeBuilder, method);
            }
        }
    }
}