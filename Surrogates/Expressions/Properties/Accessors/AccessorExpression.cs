using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Expressions.Properties.Accessors
{
    public class AccessorExpression<TBase>
        : Expression<TBase>
    {
        public AccessorExpression(InterferenceKind kind, IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { _kind = kind; }

        private InterferenceKind _kind;
        private PropertyAccessor _accessor;
        public AndExpression<TBase> Accessors(Action<AccessorChangeExpression<TBase>> changeAccessors)
        {
            var accessor =
                new AccessorChangeExpression<TBase>(_kind, Mapper, State);

            changeAccessors(accessor);

            //get was not set
            if((State.Properties.Accessors & PropertyAccessor.Get) != PropertyAccessor.Get)
            {
                //insert a basic Get
            }
            //set was not set
            if ((State.Properties.Accessors & PropertyAccessor.Set) != PropertyAccessor.Set)
            {
                //insert a basic set
            }
            
            State.Properties.Clear();

            return new AndExpression<TBase>(Mapper);
        }

        internal UsingInterferenceExpression<TBase> Call(PropertyAccessor accessor)
        {
            if ((_accessor & accessor) == accessor)
            {
                throw new AccessorAlreadyOverridenException(accessor);
            }
            return new UsingInterferenceExpression<TBase>(_kind, accessor, Mapper, State);
        }
    }
}