using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
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

        protected override MethodBuilder OverrideGetter(Property property, MethodInfo newMethod)
        {
            var pType =
                property.Original.PropertyType;

            var getter = CreateGetter(property.Original);

            ILGenerator gen = getter.GetILGenerator();

            var returnField =
                gen.DeclareLocal(pType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TSubstitutor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p => EmitPropertyNameAndField(property, pType, gen, p));

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            // in case the new method does not have return or is not assignable from property type
            if (!newMethod.ReturnType.IsAssignableFrom(pType))
            {
                if (newMethod.ReturnType != typeof(void))
                { gen.Emit(OpCodes.Pop); }

                gen.EmitDefaultValue(pType, returnField);
            }

            gen.Emit(OpCodes.Ret);

            return getter;
        }

        protected override MethodBuilder OverrideSetter(Property property, MethodInfo newMethod)
        {
            var pType =
                property.Original.PropertyType;

            var setter = CreateSetter(property.Original);

            ILGenerator gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Nop);            
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TSubstitutor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p => EmitPropertyNameAndField(property, pType, gen, p));

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            if (!newMethod.ReturnType.IsAssignableFrom(pType))
            { 
                gen.Emit(OpCodes.Stfld, property.Field); 
            }
            else if (newMethod.ReturnType != typeof(void))
            { 
                gen.Emit(OpCodes.Pop); 
            }

            gen.Emit(OpCodes.Ret);

            return setter;
        }
    }
}