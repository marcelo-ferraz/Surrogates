using Surrogates.Tactics;

namespace Surrogates.Expressions.Accessors
{
    public class AndExpression
    {
        private Strategy.ForProperties _strategy;
        public AndExpression(Strategy.ForProperties strategy)
        {
            _strategy = strategy;
        }

        public Accessors.ModifierExpression And
        {
            get { return new Accessors.ModifierExpression(_strategy); }
        }
    }
}
