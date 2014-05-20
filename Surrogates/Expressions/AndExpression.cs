using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Surrogates.Expressions
{
    public class AndExpression<TBase>
        : Expression<TBase, TBase>
    {
        internal AndExpression(IMappingExpression<TBase> mapper)
            : base(mapper)
        { }
    }

    public class AndExpression<TBase, TInstance>
        : Expression<TBase, TInstance>
    {
        internal AndExpression(IMappingExpression<TBase> mapper)
            : base(mapper)
        { }

        public IMappingExpression<TBase> And { get { return Mapper; } }
    }
}
