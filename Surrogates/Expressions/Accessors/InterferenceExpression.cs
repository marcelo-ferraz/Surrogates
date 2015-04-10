using Surrogates.Tactics;
using System;

namespace Surrogates.Expressions.Accessors
{
    public class InterferenceExpression<TBase> : Expression<TBase, Strategy.ForProperties>
    {
        public InterferenceExpression(BaseContainer4Surrogacy container, Strategy.ForProperties current, Strategies strategies)
            : base(container, current, strategies)
        { }

        public AndExpression<TBase> Accessors(Action<ModifierExpression> modExpr)
        {
            var expression = new ModifierExpression(CurrentStrategy);

            modExpr(expression);

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(Container, new Strategy(Strategies), Strategies);
        }
    }
}
