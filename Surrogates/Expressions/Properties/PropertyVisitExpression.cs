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
    public class PropertyVisitExpression<TBase, TVisitor>
        : PropertyInterferenceExpression<TBase, TVisitor>
    {
        internal PropertyVisitExpression(PropertyAccessor accessor, IMappingExpression<TBase> mapper, MappingState state)
            : base(InterferenceKind.Visitation,accessor, mapper, state) { }

        protected override MethodBuilder OverrideGetter(Property property, MethodInfo newMethod)
        {
            var pType =
                property.Original.PropertyType;

            var prop = property.Original;

            var getter = CreateGetter(prop);

            ILGenerator gen = getter.GetILGenerator();

            var returnField =
                gen.DeclareLocal(pType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TVisitor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p => EmitParameterNameAndField(property, pType, gen, p));

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            if (newMethod.ReturnType != typeof(void))
            {
                gen.Emit(OpCodes.Pop);
            }

            gen.Emit(OpCodes.Call, prop.GetMethod);
            
            gen.Emit(OpCodes.Ret);

            return getter;
        }

        protected override MethodBuilder OverrideSetter(Property property, MethodInfo newMethod)
        {
            var pType =
                property.Original.PropertyType;

            var prop = property.Original;

            var setter = CreateSetter(prop);

            ILGenerator gen = setter.GetILGenerator();

            var returnField =
                gen.DeclareLocal(pType);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, GetField4<TVisitor>());

            var @params = gen.EmitParameters4<TBase>(
                newMethod,
                p => EmitParameterNameAndField(property, pType, gen, p));

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            if (newMethod.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

            gen.Emit(OpCodes.Call, prop.GetSetMethod());

            gen.Emit(OpCodes.Ret);

            return setter;
        }
    }
}