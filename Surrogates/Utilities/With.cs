﻿using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.OldExpressions;
using Surrogates.Mappers;
using Surrogates.Tactics;

namespace Surrogates.Utilities
{
    /// <summary>
    /// Adds to the expression
    /// </summary>
    public static class With
    {
        /// <summary>
        /// It adds a very simple getter accessor to the property 
        /// </summary>
        /// <param name="expression">The expression used for changing the accessor</param>
        /// <param name="prop">The original property </param>
        public static MethodBuilder OneSimpleGetter(Strategy.ForProperties strategy, Property prop)
        {
            MethodBuilder getter = strategy.TypeBuilder.DefineMethod(
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

            return getter;
        }

        /// <summary>
        /// It adds a very simple setter accessor to the property 
        /// </summary>
        /// <param name="strategy">The expression used for changing the accessor</param> 
        /// <param name="prop">The original property </param>
        public static MethodBuilder OneSimpleSetter(Strategy.ForProperties strategy, Property prop)
        {
            //insert a basic set
            MethodBuilder setter = strategy.TypeBuilder.DefineMethod(
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

            return setter;
        }
    }
}
