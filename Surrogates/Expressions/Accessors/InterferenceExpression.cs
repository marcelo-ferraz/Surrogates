using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;

namespace Surrogates.Expressions.Accessors
{
    public class InterferenceExpression<TBase> : Expression<TBase, Strategy>
    {
        public InterferenceExpression(Strategy current, Strategies strategies)
            : base(current, strategies) { }

        public AndExpression<TBase> Accessors(Action<ModifierExpression> modExpr)
        {
            var expression = new ModifierExpression(
                new Strategy.ForProperties(CurrentStrategy));

            modExpr(expression);

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(new Strategy(Strategies), Strategies);
        }
    }
}
