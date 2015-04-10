using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class VisitExpression<TBase>
      : InterferenceExpression<TBase, Accessors.InterferenceExpression<TBase>, UsingInterferenceExpression<TBase>>
    {
        public VisitExpression(BaseContainer4Surrogacy container, Strategy current, Strategies strategies)
            : base(container, current, strategies) { }
    }
}
