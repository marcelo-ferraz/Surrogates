using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Surrogates.Tactics;
using Surrogates.Model;

namespace Surrogates.Expressions.Accessors
{
    public class InterferenceExpression<TBase> : Expression<TBase, Strategy>
    {
        private Property _property;

        public InterferenceExpression(Strategy current, Strategies strategies, Property property)
            : base(current, strategies) 
        { 
            _property = property;
        }

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
