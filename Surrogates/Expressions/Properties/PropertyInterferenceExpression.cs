using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Mappers;

namespace Surrogates.Expressions.Properties
{
    public class PropertyInterferenceExpression<TBase>
        : Expression<TBase>
    {
        internal PropertyInterferenceExpression(InterferenceKind kind, PropertyAccessor accessor, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { }

        private InterferenceKind _interferenceKind;
        private PropertyAccessor _propertyAccessor;

        public EndExpression<TBase, T> Using<T>()
        {
            return _interferenceKind == InterferenceKind.Substitution ?
                (EndExpression<TBase, T>)
                new PropertyReplaceExpression<TBase, T>(_propertyAccessor, Mapper, State) :
                new PropertyVisitExpression<TBase, T>(_propertyAccessor, Mapper, State);
        }
    }
}