using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Surrogates.Expressions
{
    public abstract class EndExpression<TBase, TInstance>
        : FluentExpression<AndExpression<TBase, TInstance>, TBase, TInstance>
    {
        internal EndExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state)
        { }

        protected override AndExpression<TBase, TInstance> Return()
        {
            return new AndExpression<TBase, TInstance>(Mapper);
        }
    }
}