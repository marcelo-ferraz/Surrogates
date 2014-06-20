
using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Properties.Accessors;
using Surrogates.Mappers;
using Surrogates.Mappers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates.Expressions.Properties
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
