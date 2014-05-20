using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Expressions.Properties
{
    public class PropertyVisitExpression<TBase, TSubstitutor>
        : EndExpression<TBase, TSubstitutor>
    {
        protected PropertyAccessor Accessor;

        internal PropertyVisitExpression(PropertyAccessor kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            Accessor = kind;
        }
        
        private void Register(Func<TSubstitutor, Delegate> action)
        {
            var method =
                action(NotInitializedInstance)
                .Method;

            var okForSetter = Accessor != PropertyAccessor.Get;
            var okForGetter = Accessor != PropertyAccessor.Set;

            foreach (var prop in State.Properties)
            {
                var newProp = State.TypeBuilder.DefineProperty(
                    prop.Name,
                    prop.Attributes,
                    prop.PropertyType,
                    null);

                if (okForGetter)
                {
                    newProp.SetSetMethod(
                        OverrideGetter(prop, method));
                }

                if (okForSetter)
                {
                    newProp.SetSetMethod(
                        OverrideSetter(prop, method));
                }
            }
            State.Properties.Clear();
        }

        private MethodBuilder OverrideGetter(PropertyInfo property, MethodInfo newMethod)
        {
            MethodBuilder getter = State.TypeBuilder.DefineMethod(
               string.Concat("get_", property.Name),
               MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
               property.PropertyType,
               Type.EmptyTypes);

            ILGenerator gen = getter.GetILGenerator();

            var returnField =
                gen.DeclareLocal(property.ReflectedType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TSubstitutor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p =>
                {
                    if (p.Name == "propertyName" && p.ParameterType == typeof(string))
                    { gen.Emit(OpCodes.Ldstr, property.Name); }
                });

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            if (newMethod.ReturnType != typeof(void))
            {
                gen.Emit(OpCodes.Pop);
            }
            
            gen.Emit(OpCodes.Call, property.GetMethod);
            
            gen.Emit(OpCodes.Ret);

            return getter;
        }

        private MethodBuilder OverrideSetter(PropertyInfo property, MethodInfo newMethod)
        {
            MethodBuilder setter = State.TypeBuilder.DefineMethod(
               string.Concat("set_", property.Name),
               MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
               typeof(void),
               Type.EmptyTypes);

            ILGenerator gen = setter.GetILGenerator();

            var returnField =
                gen.DeclareLocal(property.ReflectedType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TSubstitutor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p =>
                {
                    if (p.Name == "propertyName" && p.ParameterType == typeof(string))
                    { gen.Emit(OpCodes.Ldstr, property.Name); }
                });

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            if (newMethod.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

            gen.Emit(OpCodes.Call, property.GetSetMethod());

            gen.Emit(OpCodes.Ret);

            return setter;
        }

        protected override void RegisterAction(Func<TSubstitutor, Delegate> action)
        {
            Register(action);
        }

        protected override void RegisterFunction(Func<TSubstitutor, Delegate> function)
        {
            Register(function);
        }
    }
}