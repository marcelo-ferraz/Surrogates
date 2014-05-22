using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;

namespace Surrogates.Expressions.Properties
{
    public class PropertyInterferenceExpression<TBase, T>
        : FluentExpression<AccessorAndExpression<TBase, T>, TBase, T>
    {
        internal PropertyInterferenceExpression(InterferenceKind kind, PropertyAccessor accessor, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            Accessor = accessor;
            _kind = kind;
        }

        protected PropertyAccessor Accessor;
        private InterferenceKind _kind;

        protected override AccessorAndExpression<TBase, T> Return()
        {
            return new AccessorAndExpression<TBase,T>(_kind, Mapper, State);
        }
    }
}