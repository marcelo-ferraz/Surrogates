using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Expressions.Properties
{
    public class PropertyReplaceExpression<TBase, TSubstitutor>
        : PropertyInterferenceExpression<TBase, TSubstitutor>
    {
        internal PropertyReplaceExpression(PropertyAccessor accessor, IMappingExpression<TBase> mapper, MappingState state)
            : base(InterferenceKind.Substitution, accessor, mapper, state) { }
        
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
                    newProp.SetGetMethod(
                        OverrideGetter(prop, method));
                }

                if (okForSetter)
                {
                    newProp.SetSetMethod(
                        OverrideSetter(prop, method));
                }
            }
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
                gen.DeclareLocal(property.PropertyType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TSubstitutor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p =>
                {
                    if (p.Name == "propertyName" && p.ParameterType == typeof(string))
                    { 
                        gen.Emit(OpCodes.Ldstr, property.Name);
                        return true;
                    }
                    return false;
                });

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            // in case the new method does not have return or is not assignable from property type
            if (!newMethod.ReturnType.IsAssignableFrom(property.PropertyType))
            {
                gen.Emit(OpCodes.Pop);
                gen.EmitDefaultValue(property.PropertyType, returnField);
            }
            else if (newMethod.ReturnType == typeof(void))
            {
                gen.EmitDefaultValue(property.PropertyType, returnField);
            }

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

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TSubstitutor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p =>
                {
                    if (p.Name == "propertyName" && p.ParameterType == typeof(string))
                    { 
                        gen.Emit(OpCodes.Ldstr, property.Name);
                        return true;
                    }
                    return false;
                });

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            if (newMethod.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

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