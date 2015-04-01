using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ApplyExpression<TBase>
      : Expression<TBase, Strategy>
    {
        public Strategies Strategies { get; set; }
        public Strategy CurrentStrategy { get; set; }

        public ApplyExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }
    }
}
