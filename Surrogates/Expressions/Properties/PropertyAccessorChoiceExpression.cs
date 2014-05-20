using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Expressions.Properties
{
    public class PropertyAccessorChoiceExpression<TBase>
        : Expression<TBase>
    {
        public PropertyAccessorChoiceExpression(InterferenceKind kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { _kind = kind; }

        private InterferenceKind _kind;

        public PropertyInterferenceExpression<TBase> Get
        {
            get { return new PropertyInterferenceExpression<TBase>(_kind, PropertyAccessor.Get, Mapper, State); }
        }

        public PropertyInterferenceExpression<TBase> Set
        {
            get { return new PropertyInterferenceExpression<TBase>(_kind, PropertyAccessor.Set, Mapper, State); }
        }

        public PropertyInterferenceExpression<TBase> BothGetAndSet
        {
            get { return new PropertyInterferenceExpression<TBase>(_kind, PropertyAccessor.Both, Mapper, State); }
        }
    }
}
