﻿using Surrogates.Model;
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
        [TargetedPatchingOptOut("")]
        protected void VisitAction(MethodInfo baseAction, Strategy.ForMethods strategy)
        {
            LocalBuilder baseMethodReturn = null;

            var gen = strategy.Override( 
                baseAction, out baseMethodReturn);

            gen.Emit(OpCodes.Ldarg_0);

            var @params =
                gen.EmitParametersForSelf(strategy, baseAction);

            gen.Emit(OpCodes.Call, baseAction);
            gen.Emit(OpCodes.Ret);
        }

        [TargetedPatchingOptOut("")]
        protected void VisitFunction(MethodInfo baseFunction, Strategy.ForMethods strategy)
        {
            LocalBuilder baseMethodReturn = null;

            var gen = strategy.Override(
                baseFunction, out baseMethodReturn);
            
            gen.Emit(OpCodes.Pop);

            gen.Emit(OpCodes.Ldarg_0);

            var @params =
                gen.EmitParametersForSelf(strategy, baseFunction);

            gen.Emit(OpCodes.Call, baseFunction);

            gen.Emit(OpCodes.Ret);
        }

        protected void VisitGetter(SurrogatedProperty property, Strategy.ForProperties strategy)
        {
            var pType =
                property.Original.PropertyType;

            var prop = property.Original;

            var getter = CreateGetter(strategy, prop);

            ILGenerator gen = getter.GetILGenerator();

            var result =
                gen.DeclareLocal(pType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField(strategy.Getter, strategy.Fields));

            var @params = gen.EmitParameters(
                strategy,
                strategy.Getter,
                property.Original.GetGetMethod(),
                (p, i) => property.EmitPropertyNameAndField(gen, p));

            gen.EmitCall(strategy.Getter.Method, @params);

            if (strategy.Getter.Method.ReturnType != typeof(void))
            {
                gen.Emit(OpCodes.Pop);
            }

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, prop.GetGetMethod());

            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, result);

            gen.Emit(OpCodes.Ldloc_0);

            gen.Emit(OpCodes.Ret);

            property.Builder.SetGetMethod(getter);
        }

        protected void VisitSetter(SurrogatedProperty property, Strategy.ForProperties strategy)
        {
            var pType =
                property.Original.PropertyType;

            var prop = property.Original;

            var setter = CreateSetter(strategy, prop);

            ILGenerator gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField(strategy.Setter, strategy.Fields));

            var @params = gen.EmitParameters(
                strategy,
                strategy.Setter,
                property.Original.GetSetMethod(),
                (p, i) => property.EmitPropertyNameAndFieldAndValue(gen, p, i));

            gen.EmitCall(strategy.Setter.Method, @params);

            if (strategy.Setter.Method.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, prop.GetSetMethod());

            gen.Emit(OpCodes.Ret);

            property.Builder.SetSetMethod(setter);
        }

        public override void Execute4Properties(Strategy.ForProperties strategy)
        {
            foreach (var property in strategy.Properties)
            {
                if (strategy.Getter != null)
                { VisitGetter(property, strategy); }
                else
                { Set4Property.OneSimpleGetter(strategy, property); }

                if (strategy.Setter != null)
                { VisitSetter(property, strategy); }
                else
                { Set4Property.OneSimpleSetter(strategy, property); }     
            }
        }

        public override void Execute4Methods(Strategy.ForMethods strategy)
        {
            foreach (var method in strategy.Methods)
            {
                if (strategy.Interceptor.Method.ReturnType == typeof(void))
                { VisitAction(method, strategy); }
                else
                { VisitFunction(method, strategy); }
            }
        }
    }
}
