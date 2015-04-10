using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ReplaceExpression<TBase>
      : InterferenceExpression<TBase, Accessors.InterferenceExpression<TBase>, UsingInterferenceExpression<TBase>>
    {
        public ReplaceExpression(BaseContainer4Surrogacy container, Strategy current, Strategies strategies)
            : base(container, current, strategies) { }
    }
}
