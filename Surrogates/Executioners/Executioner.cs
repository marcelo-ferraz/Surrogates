using Surrogates.Model;
using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Utilities.Mixins;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Executioners
{
    public abstract class Executioner
    {
        protected abstract void OverrideWithAction(MethodInfo method, Strategy strategy, Strategy.InterceptorInfo interceptor, OverridenMethod overriden);

        protected abstract void OverrideWithFunction(MethodInfo method, Strategy strategy, Strategy.InterceptorInfo interceptor, OverridenMethod overriden);

        protected virtual void Execute4Property(Strategy strategy, Strategy.InterceptorInfo interceptor, SurrogatedProperty prop, bool isGetter)
        {
            var baseMethod = isGetter ?
                prop.Original.GetGetMethod() :
                prop.Original.GetSetMethod();

            var overriden = strategy.Override(
                interceptor,
                baseMethod,
                (ov, p, i) =>
                    prop.EmitPropertyNameAndFieldAndValue(ov.Generator, p, i)
                    ||
                    ov.Generator.EmitArgumentsBasedOnOriginal(baseMethod, p, i, strategy.BaseMethods.Field)
                    );


            if (interceptor.Method.ReturnType == typeof(void))
            { OverrideWithAction(baseMethod, strategy, interceptor, overriden); }
            else
            { OverrideWithFunction(baseMethod, strategy, interceptor, overriden); }

            if (isGetter)
            { prop.Builder.SetGetMethod(overriden.Builder); }
            else
            { prop.Builder.SetSetMethod(overriden.Builder); }
        }
                
        protected virtual void Execute4Properties(Strategy.ForProperties strategy)
        {
            foreach (var property in strategy.Properties)
            {
                if (strategy.Getter != null)
                { Execute4Property(strategy, strategy.Getter, property, true); }                

                if (strategy.Setter != null)
                { Execute4Property(strategy, strategy.Setter, property, false); }                
            }
        }

        protected virtual void Execute4Methods(Strategy.ForMethods strategy)
        {
            foreach (var baseMethod in strategy.Methods)
            {
                var overriden = strategy.Override(
                strategy.Interceptor,
                baseMethod,
                (ov, p, i) =>
                    ov.Generator.EmitArgumentsBasedOnOriginal(baseMethod, p, i, strategy.BaseMethods.Field));

                if (strategy.Interceptor.Method.ReturnType == typeof(void))
                { OverrideWithAction(baseMethod, strategy, strategy.Interceptor, overriden); }
                else
                { OverrideWithFunction(baseMethod, strategy, strategy.Interceptor, overriden); }
            }
        }       

        internal static MethodBuilder CreateGetter(Strategy.ForProperties strategy, PropertyInfo prop)
        {
            return strategy.TypeBuilder.DefineMethod(
                string.Concat("get_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                prop.PropertyType,
                Type.EmptyTypes);
        }

        internal static MethodBuilder CreateSetter(Strategy.ForProperties strategy, PropertyInfo prop)
        {
            return strategy.TypeBuilder.DefineMethod(
                string.Concat("set_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(void),
                new Type[] { prop.PropertyType });
        }

        public void Execute(Strategy st)
        {
            if (st is Strategy.ForProperties)
            { Execute4Properties((Strategy.ForProperties)st); }
            else
            { Execute4Methods((Strategy.ForMethods)st); }
        }
    }
}
