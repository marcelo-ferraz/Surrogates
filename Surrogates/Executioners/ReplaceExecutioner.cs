using Surrogates.Model;
using Surrogates.Tactics;
using Surrogates.Utilities;
using Surrogates.Utilities.Mixins;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;

namespace Surrogates.Executioners
{
    public class ReplaceExecutioner: Executioner
    {
        [TargetedPatchingOptOut("")]
        protected static void ReplaceAction(MethodInfo baseAction, Strategy.ForMethods strategy)
        {
            LocalBuilder baseMethodReturn = null;

            var gen = strategy.TypeBuilder.EmitOverride(
                strategy.BaseType, strategy.Interceptor.Method, baseAction, GetField(strategy.Interceptor, strategy.Fields), strategy.Fields, out baseMethodReturn);

            if (baseMethodReturn != null)
            { gen.EmitDefaultValue(baseAction.ReturnType, baseMethodReturn); }

            gen.Emit(OpCodes.Ret);
        }

        [TargetedPatchingOptOut("")]
        protected static void ReplaceFunction(MethodInfo baseFunction, Strategy.ForMethods strategy)
        {
            LocalBuilder baseMethodReturnBuilder = null;

            var gen = strategy.TypeBuilder.EmitOverride(
                strategy.BaseType,
                strategy.Interceptor.Method,
                baseFunction,
                GetField(strategy.Interceptor, strategy.Fields),
                strategy.Fields,
                out baseMethodReturnBuilder);

            //the base method is void, discard the value
            if (baseMethodReturnBuilder == null)
            {
                gen.Emit(OpCodes.Pop);
            }
            else if (!strategy.Interceptor.Method.ReturnType.IsAssignableFrom(baseFunction.ReturnType))
            {
                gen.EmitDefaultValue(strategy.Interceptor.Method.ReturnType, baseMethodReturnBuilder);
            }

            gen.Emit(OpCodes.Ret);
        }

        [TargetedPatchingOptOut("")]
        protected static void ReplaceGetter(SurrogatedProperty property, Strategy.ForProperties strategy)
        {
            var pType =
                property.Original.PropertyType;

            var getter = CreateGetter(strategy, property.Original);

            ILGenerator gen = getter.GetILGenerator();

            var returnField =
                gen.DeclareLocal(pType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField(strategy.Getter, strategy.Fields));

            var @params = gen.EmitParameters(
                strategy.BaseType,
                strategy.Fields,
                strategy.Getter.Method,
                p => property.EmitPropertyNameAndField(pType, gen, p));

            gen.EmitCall(strategy.Getter.Method, @params);

            // in case the new method does not have return or is not assignable from property type
            if (!strategy.Getter.Method.ReturnType.IsAssignableFrom(pType))
            {
                if (strategy.Getter.Method.ReturnType != typeof(void))
                { gen.Emit(OpCodes.Pop); }

                gen.EmitDefaultValue(pType, returnField);
            }

            gen.Emit(OpCodes.Ret);

            property.Builder.SetGetMethod(getter);
        }

        [TargetedPatchingOptOut("")]
        protected static MethodBuilder ReplaceSetter(SurrogatedProperty property, Strategy.ForProperties strategy)
        {
            var pType =
                property.Original.PropertyType;

            var setter = CreateSetter(strategy, property.Original);

            ILGenerator gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField(strategy.Setter, strategy.Fields));

            var @params = gen.EmitParameters(
                strategy.BaseType,
                strategy.Fields,
                strategy.Setter.Method,
                p => property.EmitPropertyNameAndField(pType, gen, p));

            gen.EmitCall(strategy.Setter.Method, @params);

            if (strategy.Setter.Method.ReturnType != typeof(void) &&
                !strategy.Setter.Method.ReturnType.IsAssignableFrom(pType))
            {
                gen.Emit(OpCodes.Stfld, property.Field);
            }
            else if (strategy.Setter.Method.ReturnType != typeof(void))
            {
                gen.Emit(OpCodes.Pop);
            }

            gen.Emit(OpCodes.Ret);

            return setter;
        }

        public override void Execute4Properties(Strategy.ForProperties strategy)
        {
            foreach (var property in strategy.Properties)
            {
                if(strategy.Getter != null) 
                { ReplaceGetter(property, strategy); }
                else
                { Set4Property.OneSimpleGetter(strategy, property); }

                if (strategy.Setter != null)
                { ReplaceSetter(property, strategy); }
                else
                { Set4Property.OneSimpleSetter(strategy, property); }                
            }
        }

        public override void Execute4Methods(Strategy.ForMethods strategy)
        {
            foreach (var method in strategy.Methods)
            {
                if (strategy.Interceptor.Method.ReturnType == typeof(void))
                { ReplaceAction(method, strategy); }
                else
                { ReplaceFunction(method, strategy); }
            }
        }
    }
}
