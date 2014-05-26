using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Expressions.Properties.Accessors
{
    public class AccessorExpression<TBase>
        : Expression<TBase>
    {
        public AccessorExpression(InterferenceKind kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { _kind = kind; }

        private InterferenceKind _kind;

        private void EmitDefaultSet(Property prop)
        {
            if (prop.Field == null)
            {
                prop.Field =
                    State
                    .TypeBuilder
                    .DefineFieldFromProperty(prop.Original);
            }

            //insert a basic set
            MethodBuilder setter = State.TypeBuilder.DefineMethod(
                string.Concat("set_", prop.Original.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(void),
                new[] { prop.Original.PropertyType });

            ILGenerator gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, prop.Field);
            gen.Emit(OpCodes.Ret);

            prop.Builder.SetSetMethod(setter);
        }

        private void EmitDefaultGet(Property prop)
        {
            if (prop.Field == null)
            {
                prop.Field =
                    State
                    .TypeBuilder
                    .DefineFieldFromProperty(prop.Original);
            }

            MethodBuilder getter = State.TypeBuilder.DefineMethod(
                string.Concat("get_", prop.Original.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                prop.Original.PropertyType,
                Type.EmptyTypes);

            ILGenerator gen = getter.GetILGenerator();

            var local =
                gen.DeclareLocal(prop.Original.PropertyType);

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, prop.Field);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, local);

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            prop.Builder.SetGetMethod(getter);
        }
 
        public AndExpression<TBase> Accessors(Action<AccessorChangeExpression<TBase>> changeAccessors)
        {
            var accessor =
                new AccessorChangeExpression<TBase>(_kind, Mapper, State);

            changeAccessors(accessor);

            //get was not set
            if ((State.Properties.Accessors & PropertyAccessor.Get) != PropertyAccessor.Get)
            {
                foreach (var prop in State.Properties)
                {
                    EmitDefaultGet(prop);
                }
            }
            //set was not set
            if ((State.Properties.Accessors & PropertyAccessor.Set) != PropertyAccessor.Set)
            {
                foreach (var prop in State.Properties)
                {
                    EmitDefaultSet(prop);
                }
            }

            State.Properties.Clear();

            return new AndExpression<TBase>(Mapper);
        }
   }
}