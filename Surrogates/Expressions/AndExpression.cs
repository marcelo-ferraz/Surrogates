using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class AndExpression<TBase> : Expression<TBase, Strategy>
    {
        public AndExpression(BaseContainer4Surrogacy container, Strategy current, Strategies strategies)
            : base(container, current, strategies) { }

        public ExpressionFactory<TBase> And 
        {
            get { return new ExpressionFactory<TBase>(Container, CurrentStrategy, Strategies); }
        }
    }
}
