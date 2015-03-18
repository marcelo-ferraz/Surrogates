using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Text;
using Surrogates.Utilities.Mixins;
using Surrogates.Mappers;
using Surrogates.Utilities;

namespace Surrogates.Executioners
{
    public class VisitationExecutioner : Executioner
    {
        [TargetedPatchingOptOut("")]
        protected void VisitAction(MethodInfo baseAction, Strategy.ForMethods strategy)
        {
            LocalBuilder baseMethodReturn = null;

            var gen = strategy.TypeBuilder.EmitOverride(
                strategy.BaseType,
                strategy.Interceptor, baseAction, GetField(strategy), out baseMethodReturn);

            gen.Emit(OpCodes.Ldarg_0);

            var @params =
                gen.EmitParameters(strategy.BaseType, strategy.Interceptor, baseAction);

            gen.Emit(OpCodes.Call, baseAction);
            gen.Emit(OpCodes.Ret);
        }

        [TargetedPatchingOptOut("")]
        protected void VisitFunction(MethodInfo baseFunction, Strategy.ForMethods strategy)
        {
            LocalBuilder baseMethodReturn = null;

            var gen = strategy.TypeBuilder.EmitOverride(
                strategy.BaseType,
                strategy.Interceptor,
                baseFunction,
                GetField(strategy),
                out baseMethodReturn);

            gen.Emit(OpCodes.Pop);

            gen.Emit(OpCodes.Ldarg_0);

            var @params =
                gen.EmitParameters(strategy.BaseType, strategy.Interceptor, baseFunction);

            gen.Emit(OpCodes.Call, baseFunction);

            gen.Emit(OpCodes.Ret);
        }

        protected MethodBuilder VisitGetter(Property property, Strategy.ForProperties strategy)
        {
            var pType =
                property.Original.PropertyType;

            var prop = property.Original;

            var getter = CreateGetter(strategy, prop);

            ILGenerator gen = getter.GetILGenerator();

            var result =
                gen.DeclareLocal(pType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField(strategy));

            var @params = gen.EmitParameters(
                strategy.BaseType,
                strategy.Getter,
                p => property.EmitPropertyNameAndField(pType, gen, p));

            gen.EmitCall(strategy.Getter, @params);

            if (strategy.Getter.ReturnType != typeof(void))
            {
                gen.Emit(OpCodes.Pop);
            }

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, prop.GetGetMethod());

            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, result);

            gen.Emit(OpCodes.Ldloc_0);

            gen.Emit(OpCodes.Ret);

            return getter;
        }

        protected MethodBuilder VisitSetter(Property property, Strategy.ForProperties strategy)
        {
            var pType =
                property.Original.PropertyType;

            var prop = property.Original;

            var setter = CreateSetter(strategy, prop);

            ILGenerator gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField(strategy));

            var @params = gen.EmitParameters(
                strategy.BaseType,
                strategy.Setter,
                p => property.EmitPropertyNameAndFieldAndValue(pType, gen, p));

            gen.EmitCall(strategy.Setter, @params);

            if (strategy.Setter.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, prop.GetSetMethod());

            gen.Emit(OpCodes.Ret);

            return setter;
        }

        public override void Execute4Properties(Strategy.ForProperties strategy)
        {
            foreach (var property in strategy.Properties)
            {
                var getter = strategy.Getter != null ?
                    VisitGetter(property, strategy) :
                    With.OneSimpleGetter(strategy, property);

                var setter = strategy.Setter != null ?
                    VisitSetter(property, strategy) :
                    With.OneSimpleSetter(strategy, property);

                property.Builder.SetGetMethod(getter);
                property.Builder.SetSetMethod(setter);
            }
        }

        public override void Execute4Methods(Strategy.ForMethods strategy)
        {
            foreach (var method in strategy.Methods)
            {
                if (method.ReturnType == typeof(void))
                { VisitAction(method, strategy); }
                else
                { VisitFunction(method, strategy); }
            }
        }
    }
}
