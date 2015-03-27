using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class AndExpression<TBase> : Expression<TBase, Strategy>
    {
        public AndExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }

        public ExpressionFactory<TBase> And 
        {
            get { return new ExpressionFactory<TBase>(CurrentStrategy, Strategies); }
        }
    }
}
