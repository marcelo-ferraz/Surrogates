using Surrogates.Tactics;

namespace Surrogates.Expressions
{
    public class ExpressionFactory<TBase> : Expression<TBase, Strategy>
    {
        internal ExpressionFactory(Strategy current, Strategies strategies)
            : base(current, strategies)
        { }

        public ReplaceExpression<TBase> Replace
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Replace;
                return new ReplaceExpression<TBase>(CurrentStrategy, Strategies);
            }
        }

        public DisableExpression<TBase> Disable
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Disable;
                return new DisableExpression<TBase>(CurrentStrategy, Strategies);
            }
        }

        public VisitExpression<TBase> Visit
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Visit;
                return new VisitExpression<TBase>(CurrentStrategy, Strategies);
            }
        }

        public ApplyExpression<TBase> Apply
        {
            get
            {
                CurrentStrategy.Kind = InterferenceKind.Extensions;
                return new ApplyExpression<TBase>(CurrentStrategy, Strategies);
            }
        }
    }
}
