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
        { Kind = kind; }

        internal InterferenceKind Kind { get; set; }

        private void EmitDefaultSet(Property prop)
        {
            //insert a basic set
            MethodBuilder setter = State.TypeBuilder.DefineMethod(
                string.Concat("set_", prop.Original.Name),
                MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                typeof(void),
                new[] { prop.Original.PropertyType });

            ILGenerator gen = setter.GetILGenerator();

            var originalSetter =
                prop.Original.GetSetMethod();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, originalSetter);

            if (originalSetter.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

            gen.Emit(OpCodes.Ret);

            prop.Builder.SetSetMethod(setter);
        }

        public void EmitBaseGetter(Property prop)
        {
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
            gen.Emit(OpCodes.Call, prop.Original.GetGetMethod());
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, local);

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            prop.Builder.SetGetMethod(getter);
        }


        public AndExpression<TBase> Accessors(Action<AccessorChangeExpression<TBase>> changeAccessors)
        {
            var accessor =
                new AccessorChangeExpression<TBase>(Kind, Mapper, State);

            changeAccessors(accessor);

            //get was not set
            if ((State.Properties.Accessors & PropertyAccessor.Get) != PropertyAccessor.Get)
            {
                foreach (var prop in State.Properties)
                {
                    EmitBaseGetter(prop);
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