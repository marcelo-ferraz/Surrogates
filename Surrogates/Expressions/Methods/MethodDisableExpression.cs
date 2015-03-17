using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogates.Mappers.Entities;
using Surrogates.Utilities.Mixins;

namespace Surrogates.Expressions.Methods
{
    public class MethodDisableExpression<TBase>
        : FluentExpression<MethodDisableExpression<TBase>, TBase, TBase>
    {
        internal MethodDisableExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state) { }

        protected override void Register(MethodInfo method)
        {
            var builder = State.TypeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType,
                method.GetParameters().Select(p => p.ParameterType).ToArray());

            var gen = builder.GetILGenerator();

            if (method.ReturnType != typeof(void))
            {
                gen.EmitDefaultValue(method.ReturnType);
            }
            gen.Emit(OpCodes.Ret);
        }
    }
}