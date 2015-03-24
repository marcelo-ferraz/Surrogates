
using Surrogates.OldExpressions.Properties.Accessors;
using Surrogates.Mappers.Entities;

namespace Surrogates.OldExpressions.Properties
{
    public class UsingInterferenceExpression<TBase>
        : Expression<TBase>
    {
        internal UsingInterferenceExpression(InterferenceKind kind, PropertyAccessor accessor, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        {
            _interferenceKind = kind;
            _propertyAccessor = accessor;
        }

        private InterferenceKind _interferenceKind;
        private PropertyAccessor _propertyAccessor;

        /// <summary>
        /// Exposes the interference type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public PropertyInterferenceExpression<TBase, T> Using<T>(string fieldName = null)
        {
            State.Properties.Add(_propertyAccessor);

            return _interferenceKind == InterferenceKind.Substitution ?
                (PropertyInterferenceExpression<TBase, T>)
                new PropertyReplaceExpression<TBase, T>(_propertyAccessor, Mapper, State, fieldName) :
                new PropertyVisitExpression<TBase, T>(_propertyAccessor, Mapper, State, fieldName);
        }
    }
}
