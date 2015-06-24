using Surrogates.Tactics;
using System;

namespace Surrogates.Expressions.Accessors
{
    public class InterferenceExpression<TBase> : Expression<TBase, Strategy.ForProperties>
    {
        public InterferenceExpression(BaseContainer4Surrogacy container, Strategy.ForProperties current, Strategies strategies)
            : base(container, current, strategies)
        { }

        /// <summary>
        /// Opens a series of expressions to be used to modify the accessors of the provided properties
        /// </summary>
        /// <param name="modExpr">The expression that facilitates the modification of the acessors</param>
        /// <returns></returns>
        public AndExpression<TBase> Accessors(Action<ModifierExpression> modExpr)
        {
            var expression = 
                new ModifierExpression(CurrentStrategy);

            modExpr(expression);

            Strategies.Add(CurrentStrategy);

            return new AndExpression<TBase>(Container, new Strategy(Strategies), Strategies);
        }
    }
}
