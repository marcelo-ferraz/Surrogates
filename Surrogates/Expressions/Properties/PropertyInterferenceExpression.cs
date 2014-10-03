using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Properties
{
    public class PropertyInterferenceExpression<TBase, T>
        : FluentExpression<AccessorAndExpression<TBase, T>, TBase, T>
    {
        internal PropertyInterferenceExpression(InterferenceKind kind, PropertyAccessor accessor, IMappingExpression<TBase> mapper, MappingState state, string fieldName)
            : base(mapper, state)
        {
            Accessor = accessor;
            this._kind = kind;
            this.FieldName = fieldName;
        }

        private InterferenceKind _kind;

        protected PropertyAccessor Accessor;
        protected string FieldName;

        protected override AccessorAndExpression<TBase, T> Return()
        {
            return new AccessorAndExpression<TBase,T>(_kind, Mapper, State);
        }

        protected static bool EmitPropertyNameAndField(Property property, Type pType, ILGenerator gen, ParameterInfo p)
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


        protected static bool EmitPropertyNameAndFieldAndValue(Property property, Type pType, ILGenerator gen, ParameterInfo p)
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

            if (p.Name == "value" && p.ParameterType.IsAssignableFrom(property.Original.PropertyType))
            {
                gen.Emit(OpCodes.Ldarg_1);
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
            return State.TypeBuilder.DefineMethod(
                string.Concat("set_", prop.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(void),
                new Type[] { prop.PropertyType });
        }

        protected virtual void Register(Func<T, Delegate> action)
        {
            var method =
                action(NotInitializedInstance)
                .Method;

            foreach (var prop in State.Properties)
            {
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