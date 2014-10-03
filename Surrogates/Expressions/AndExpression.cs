using Surrogates.Expressions.Classes;

namespace Surrogates.Expressions
{
    public class AndExpression<TBase>
        : AndExpression<TBase, TBase>
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
