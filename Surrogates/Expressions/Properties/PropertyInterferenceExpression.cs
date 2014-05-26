using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Expressions.Properties
{
    public class PropertyInterferenceExpression<TBase, T>
        : FluentExpression<AccessorAndExpression<TBase, T>, TBase, T>
    {
        internal PropertyInterferenceExpression(InterferenceKind kind, PropertyAccessor accessor, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            Accessor = accessor;
            _kind = kind;
        }

        protected PropertyAccessor Accessor;
        private InterferenceKind _kind;

        protected override AccessorAndExpression<TBase, T> Return()
        {
            return new AccessorAndExpression<TBase,T>(_kind, Mapper, State);
        }

        protected static bool EmitParameterNameAndField(Property property, Type pType, ILGenerator gen, ParameterInfo p)
        {
            if (p.Name == "propertyName" && p.ParameterType == typeof(string))
            {
                gen.Emit(OpCodes.Ldstr, property.Original.Name);
                return true;
            }

            if (p.Name == "field" && p.ParameterType == pType)
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, property.Field);
                return true;
            }

            return false;
        }

        protected virtual MethodBuilder CreateGetter(PropertyInfo prop)
        {
            return State.TypeBuilder.DefineMethod(
                           string.Concat("get_", prop.Name),
                           MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                           prop.PropertyType,
                           Type.EmptyTypes);
        }

        protected MethodBuilder CreateSetter(PropertyInfo prop)
        {
            MethodBuilder setter = State.TypeBuilder.DefineMethod(
               string.Concat("set_", prop.Name),
               MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
               typeof(void),
               Type.EmptyTypes);
            return setter;
        }

        protected virtual void Register(Func<T, Delegate> action)
        {
            var method =
                action(NotInitializedInstance)
                .Method;

            foreach (var prop in State.Properties)
            {
                prop.Builder = State.TypeBuilder.DefineProperty(
                    prop.Original.Name,
                    prop.Original.Attributes,
                    prop.Original.PropertyType,
                    null);

                if ((Accessor & PropertyAccessor.Get) == PropertyAccessor.Get)
                {
                    prop.Builder.SetGetMethod(
                        OverrideGetter(prop, method));
                }

                if ((Accessor & PropertyAccessor.Set) == PropertyAccessor.Set)
                {
                    prop.Builder.SetSetMethod(
                        OverrideSetter(prop, method));
                }
            }
        }

        protected virtual MethodBuilder OverrideSetter(Property prop, MethodInfo method)
        {
            throw new NotImplementedException();
        }

        protected virtual MethodBuilder OverrideGetter(Property prop, MethodInfo method)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterAction(Func<T, Delegate> action)
        {
            Register(action);
        }

        protected override void RegisterFunction(Func<T, Delegate> function)
        {
            Register(function);
        }
    }
}