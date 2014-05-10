using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates
{
    internal static class TypeBuilderMixins
    {
        internal static void CreateConstructor4<T>(this TypeBuilder typeBuilder, IList<FieldInfo> fields)
        {
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { });

            var ctrGen = ctorBuilder.GetILGenerator();

            ctrGen.EmitConstructor4<T>(fields);
        }

        internal static ILGenerator EmitOverride(this TypeBuilder typeBuilder, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField)
        {
            var builder = typeBuilder.DefineMethod(
                baseMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                baseMethod.ReturnType,
                baseMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            var gen = builder.GetILGenerator();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, interceptorField);

            var @params =
                gen.ArrangeTheParameters(newMethod, baseMethod);

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            return gen;
        }
    }
}