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
        protected static void ReplaceWithAction(MethodInfo baseAction, Strategy.ForMethods strategy)
        {
            LocalBuilder baseMethodReturn = null;

            var gen = strategy.Override(
                baseAction, out baseMethodReturn);

            if (baseMethodReturn != null)
            { gen.EmitDefaultLocalValue(baseAction.ReturnType, ref baseMethodReturn); }

            gen.Emit(OpCodes.Ret);
        }

        [TargetedPatchingOptOut("")]
        protected static void ReplaceWithFunction(MethodInfo baseFunction, Strategy.ForMethods strategy)
        {
            LocalBuilder baseMethodReturn = null;

            var gen = strategy.Override(
                baseFunction, out baseMethodReturn);

            //the base method is void, discard the value
            if (baseMethodReturn == null)
            {
                gen.Emit(OpCodes.Pop);
            }
            else if (!strategy.Interceptor.Method.ReturnType.IsAssignableFrom(baseFunction.ReturnType))
            {
                gen.EmitDefaultLocalValue(strategy.Interceptor.Method.ReturnType, ref baseMethodReturn);
            }
            else // in case the new method's return needs to be cast 
                if (strategy.Interceptor.Method.ReturnType != baseFunction.ReturnType)
                { 
                    if(baseFunction.ReturnType.IsValueType)
                    {                        
                        gen.Emit(OpCodes.Unbox_Any, baseFunction.ReturnType);
                        gen.Emit(OpCodes.Stloc, baseMethodReturn);
                        gen.Emit(OpCodes.Ldloc, baseMethodReturn);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Castclass, baseFunction.ReturnType); 
                    }
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
                strategy,
                strategy.Getter,
                property.Original.GetGetMethod(),
                (p, i) => property.EmitPropertyNameAndField(gen, p));
            
            gen.EmitCall(strategy.Getter.Method, @params);

            // in case the new method does not have return or is not assignable from property type
            if (!strategy.Getter.Method.ReturnType.IsAssignableFrom(pType))
            {
                if (strategy.Getter.Method.ReturnType != typeof(void))
                { gen.Emit(OpCodes.Pop); }

                gen.EmitDefaultLocalValue(pType, ref returnField);
            }
            else // in case the new method's return needs to be cast 
                if (strategy.Getter.Method.ReturnType != property.Original.PropertyType)
                {
                    if (property.Original.PropertyType.IsValueType)
                    {
                        //IL_002a: unbox.any [mscorlib]System.Int32
                        gen.Emit(OpCodes.Unbox_Any, property.Original.PropertyType);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Castclass, property.Original.PropertyType); 
                    }
                }           


            gen.Emit(OpCodes.Ret);

            property.Builder.SetGetMethod(getter);
        }

        [TargetedPatchingOptOut("")]
        protected static void ReplaceSetter(SurrogatedProperty property, Strategy.ForProperties strategy)
        {
            var pType =
                property.Original.PropertyType;

            var setter = CreateSetter(strategy, property.Original);

            ILGenerator gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField(strategy.Setter, strategy.Fields));

            var @params = gen.EmitParameters(
                strategy,
                strategy.Setter,
                property.Original.GetSetMethod(),
                (p, i) => property.EmitPropertyNameAndFieldAndValue(gen, p, i));

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

            property.Builder.SetSetMethod(setter);
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
                { ReplaceWithAction(method, strategy); }
                else
                { ReplaceWithFunction(method, strategy); }
            }
        }
    }
}
