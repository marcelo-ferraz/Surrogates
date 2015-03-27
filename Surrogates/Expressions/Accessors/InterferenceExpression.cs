using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;
using Surrogates.Model;

namespace Surrogates.Expressions.Accessors
{
    public class InterferenceExpression<TBase> : Expression<TBase, Strategy.ForProperties>
    {
        public InterferenceExpression(Strategy.ForProperties current, Strategies strategies)
            : base(current, strategies)
        { }

        public AndExpression<TBase> Accessors(Action<ModifierExpression> modExpr)
        {
            var expression = new ModifierExpression(CurrentStrategy);

            modExpr(expression);

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(new Strategy(Strategies), Strategies);
        }
    }
}
