using System;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Model;

namespace Surrogates.Utilities.Mixins
{
    public static class PropertyBuilderMixns
    {
        private static readonly MethodAttributes _flags =
            MethodAttributes.Virtual | MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        internal static Property EmitDefaultSet(
            this Property that, TypeBuilder builder)
        {
            //insert a basic set
            MethodBuilder setter = builder.DefineMethod(
                string.Concat("set_", that.Original.Name),
                _flags,
                typeof(void),
                new[] { that.Original.PropertyType });

            ILGenerator gen = setter.GetILGenerator();

            var originalSetter =
                that.Original.GetSetMethod();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, originalSetter);

            if (originalSetter.ReturnType != typeof(void))
            { gen.Emit(OpCodes.Pop); }

            gen.Emit(OpCodes.Ret);

            that.Builder.SetSetMethod(setter);

            return that;
        }

        internal static Property EmitBaseGetter(
            this Property that, TypeBuilder builder)
        {
            MethodBuilder getter = builder.DefineMethod(
                string.Concat("get_", that.Original.Name),
                _flags,
                that.Original.PropertyType,
                Type.EmptyTypes);

            ILGenerator gen = getter.GetILGenerator();

            var local =
                gen.DeclareLocal(that.Original.PropertyType);

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, that.Original.GetGetMethod());
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, local);

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            that.Builder.SetGetMethod(getter);

            return that;
        }
    }
}
