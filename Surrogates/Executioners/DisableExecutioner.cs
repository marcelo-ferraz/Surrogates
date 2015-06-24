using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Executioners
{
    public class DisableExecutioner 
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
            var setter = Executioner
                .CreateSetter(strategy, property.Original);

            var gen = setter.GetILGenerator();
            gen.Emit(OpCodes.Ret);

            property.Builder.SetSetMethod(setter);
        }
        
        protected static void DisableGetter(Strategy.ForProperties strategy, Model.SurrogatedProperty property)
        {
            var getter = Executioner
                .CreateGetter(strategy, property.Original);

            var gen = getter.GetILGenerator();

            gen.EmitDefaultLocalValue(property.Original.PropertyType);
            gen.Emit(OpCodes.Ret);

            property.Builder.SetGetMethod(getter);
        }

        public void Execute(Strategy strategy)
        {
            if (strategy is Strategy.ForProperties)
            {
                var st = strategy as 
                    Strategy.ForProperties;

                foreach (var property in st.Properties)
                {
                    DisableSetter(st, property);

                    DisableGetter(st, property);
                }
            }
            else
            {
                var st = strategy as
                    Strategy.ForMethods;

                foreach (var method in st.Methods)
                {
                    DisableMethod(strategy.TypeBuilder, method);
                }
            }
        }
    }
}