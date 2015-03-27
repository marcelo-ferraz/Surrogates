using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class VisitExpression<TBase>
      : InterferenceExpression<TBase, Accessors.InterferenceExpression<TBase>, UsingInterferenceExpression<TBase>>
    {
        public VisitExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }
    }
}
