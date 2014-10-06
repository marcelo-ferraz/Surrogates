using System.Reflection.Emit;
using Surrogates.Expressions.Classes;
using Surrogates.Mappers.Entities;

namespace Surrogates.Expressions.Methods
{
    public class MethodInterferenceExpression<TBase>
     : FluentExpression<EndInterferenceExpression<TBase>, TBase, TBase>
    {
        protected TypeBuilder Typebuilder;
        protected InterferenceKind Kind;

        internal MethodInterferenceExpression(
            IMappingExpression<TBase> mapper, MappingState state, InterferenceKind kind)
            : base(mapper, state)
        {
            Kind = kind;
        }

        protected override EndInterferenceExpression<TBase> Return()
        {
            return new EndInterferenceExpression<TBase>(this.Mapper, this.State, Kind);
        }
    }
}