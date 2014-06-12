using Surrogates.Expressions;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Utils
{
    /// <summary>
    /// Adds to the expression
    /// </summary>
    public static class With
    {
        private static void EmitBasicSetter(MappingState state, Property prop)
        {
            //insert a basic set
            MethodBuilder setter = state.TypeBuilder.DefineMethod(
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

        private static void EmitBasicGetter(MappingState state, Property prop)
        {
            MethodBuilder getter = state.TypeBuilder.DefineMethod(
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

        /// <summary>
        /// It adds a very simple getter accessor to the property 
        /// </summary>
        /// <typeparam name="T">Property owner's type</typeparam>
        /// <param name="expression">The expression used for changing the accessor</param>
        public static void OneSimpleGetter<T>(AccessorChangeExpression<T> expression)
        {
            if (expression.Kind != InterferenceKind.Substitution)
            { throw new NotSupportedException("The only supported action is replacement"); }

            var state =
                expression.State;
            //get was not set
            if ((state.Properties.Accessors & PropertyAccessor.Get) != PropertyAccessor.Get)
            {
                foreach (var prop in state.Properties)
                {
                    EmitBasicGetter(state, prop);
                }
                state.Properties.Accessors |= PropertyAccessor.Get;
            }
        }

        /// <summary>
        /// It adds a very simple setter accessor to the property 
        /// </summary>
        /// <typeparam name="T">Property owner's type</typeparam>
        /// <param name="expression">The expression used for changing the accessor</param>        
        public static void OneSimpleSetter<T>(AccessorChangeExpression<T> expression)
        {
            if (expression.Kind != InterferenceKind.Substitution)
            { throw new NotSupportedException("The only supported action is replacement"); }

            var state =
                expression.State;

            if ((state.Properties.Accessors & PropertyAccessor.Set) != PropertyAccessor.Set)
            {
                foreach (var prop in state.Properties)
                {
                    EmitBasicSetter(state, prop);
                }
                state.Properties.Accessors |= PropertyAccessor.Set;
            }
        }
    }
}
